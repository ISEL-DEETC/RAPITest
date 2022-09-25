using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using RAPITest.Attributes;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System;
using ModelsLibrary.Models.EFModels;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Threading;
using RAPITest.Utils;
using ModelsLibrary.Models.AppSpecific;
using Newtonsoft.Json.Linq;
using ModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAnnotation.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class SetupTestController : Controller
	{
		private readonly ILogger<SetupTestController> _logger;
		private readonly long _fileSizeLimit;
		private readonly string[] _permittedExtensions;
		private readonly RAPITestDBContext _context;
		private readonly string RabbitMqHostName;
		private readonly int RabbitMqPort;
		private static readonly HttpClient _httpClient;
		private static readonly FormOptions _defaultFormOptions;

		static SetupTestController()
		{
			_httpClient = new HttpClient();
			_defaultFormOptions = new FormOptions();
		}
		public SetupTestController(ILogger<SetupTestController> logger, RAPITestDBContext context, IConfiguration config)
		{
			_logger = logger;
			_context = context;
			_fileSizeLimit = config.GetValue<long>("FileSizeLimit");
			_permittedExtensions = config.GetSection("PermittedExtensionsApiSpecification").GetChildren().ToArray().Select(v => v.Value).ToArray();
			RabbitMqHostName = config.GetValue<string>("RabbitMqHostName");
			RabbitMqPort = config.GetValue<int>("RabbitMqPort");
		}

		[HttpPost]
		[DisableRequestSizeLimit]
		[DisableFormValueModelBinding]
		public IActionResult UploadFile(IFormCollection data)
		{
			List<IFormFile> files = data.Files.ToList();

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			using (_context)
			{
				Api newApi = _context.Api.OrderByDescending(x => x.ApiId).FirstOrDefault();

				newApi.RunGenerated = data["rungenerated"] == "true";

				List<IFormFile> tsls = new List<IFormFile>();
				List<IFormFile> externalDlls = new List<IFormFile>();

				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						if (formFile.Name.Contains("tsl_"))
						{
							tsls.Add(formFile);
						}
						else if (formFile.Name.Contains("apiSpecification"))
						{
							using (var ms = new MemoryStream())
							{
								formFile.CopyTo(ms);
								newApi.ApiSpecification = ms.ToArray();
							}
						}
						else if (formFile.Name.Contains("dictionary"))
						{
							using (var ms = new MemoryStream())
							{
								formFile.CopyTo(ms);
								newApi.Dictionary = ms.ToArray();
							}
						}
						else
						{
							externalDlls.Add(formFile);
						}
					}
				}

				string filesConcatenated = "";
				using (var ms = new MemoryStream())
				{
					foreach (IFormFile tsl in tsls)
					{
						using (var reader = new StreamReader(tsl.OpenReadStream()))
						{
							filesConcatenated += reader.ReadToEnd();
						}
					}
				}

				newApi.Tsl = Encoding.Default.GetBytes(filesConcatenated);

				//radioButtons: [button1H, button12H, button24H, button1W, button1M, buttonNever] 
				switch (data["interval"])
				{
					case "1 hour":
						newApi.NextTest = DateTime.Now.AddHours(1);
						newApi.TestTimeLoop = 1;
						break;
					case "12 hours":
						newApi.NextTest = DateTime.Now.AddHours(12);
						newApi.TestTimeLoop = 12;
						break;
					case "24 hours":
						newApi.NextTest = DateTime.Now.AddDays(1);
						newApi.TestTimeLoop = 24;
						break;
					case "1 week":
						newApi.NextTest = DateTime.Now.AddDays(7);
						newApi.TestTimeLoop = 168;
						break;
					default:  //Never
						break;
				}
				int identityId = newApi.ApiId;

				foreach (IFormFile external in externalDlls)
				{
					ExternalDll externalDll = new ExternalDll();
					externalDll.ApiId = identityId;
					using var ms = new MemoryStream();
					external.CopyTo(ms);
					externalDll.Dll = ms.ToArray();
					externalDll.FileName = external.FileName;

					_context.ExternalDll.Add(externalDll);
				}
				_context.SaveChanges();

				Sender(identityId, data["runimmediately"] == "true");
			}
			return Created(nameof(SetupTestController), null);
		}

		[HttpPost]
		[DisableRequestSizeLimit]
		[DisableFormValueModelBinding]
		public IActionResult GetSpecificationDetails(IFormCollection data)
		{
			List<IFormFile> files = data.Files.ToList();

			IFormFile apispec = files.Single();

			APISpecificationInfo aPISpecificationInfo = GetAPISpecificationInfo.GetSpecInfo(apispec);

			if(aPISpecificationInfo.Error == null)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

				Api newApi = new Api();
				newApi.ApiTitle = data["title"];
				newApi.UserId = userId;

				using (var ms = new MemoryStream())
				{
					apispec.CopyTo(ms);
					newApi.ApiSpecification = ms.ToArray();
				}

				newApi.RunGenerated = false;
				newApi.Tsl = Encoding.Default.GetBytes("");

				using (_context)
				{
					_context.Api.Add(newApi);
					_context.SaveChanges();
				}

				return Ok(aPISpecificationInfo);
			}

			return BadRequest(aPISpecificationInfo);
		}

		[HttpPost]
		public async Task<IActionResult> GetSpecificationDetailsURL(IFormCollection data)
		{
			string url = data["apiSpecification"];
			try
			{
				Uri uri = new Uri(url);
				string finalSegment = uri.Segments[uri.Segments.Length-1];
				if (Path.GetExtension(finalSegment) != ".json" && Path.GetExtension(finalSegment) != ".yaml")
				{
					APISpecificationInfo aux = new APISpecificationInfo();
					aux.Error = "File not valid";
					return BadRequest(aux);
				}

				byte[] byteArray = await _httpClient.GetByteArrayAsync(uri);
				var stream = new MemoryStream(byteArray);
				IFormFile file = new FormFile(stream, 0, byteArray.Length, "name", "fileName");

				APISpecificationInfo aPISpecificationInfo = GetAPISpecificationInfo.GetSpecInfo(file);

				if (aPISpecificationInfo.Error == null)
				{
					var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

					Api newApi = new Api();
					newApi.ApiTitle = data["title"];
					newApi.UserId = userId;
					newApi.ApiSpecification = byteArray;
					newApi.RunGenerated = false;
					newApi.Tsl = Encoding.Default.GetBytes("");

					using (_context)
					{
						_context.Api.Add(newApi);
						_context.SaveChanges();
					}

					return Ok(aPISpecificationInfo);
				}

				return BadRequest(aPISpecificationInfo);
			}
			catch(Exception e)
			{
				APISpecificationInfo aux = new APISpecificationInfo();
				aux.Error = "File not valid";
				return BadRequest(aux);
			}
		}

		[HttpPost]
		public IActionResult RemoveUnfinishedSetup()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			using (_context)
			{
				Api newApi = _context.Api.OrderByDescending(x => x.ApiId).FirstOrDefault();

				if (newApi == null) return NotFound();

				_context.Api.Remove(newApi);
				_context.SaveChanges();
			}
			return Ok();
		}

		public void Sender(int apiId, bool runImmediately)
		{
			var factory = new ConnectionFactory() { HostName = RabbitMqHostName, Port = RabbitMqPort };   //as longs as it is running in the same machine
			using (var connection = CreateConnection(factory))
			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(queue: "setup",
									 durable: false,
									 exclusive: false,
									 autoDelete: false,
									 arguments: null);

				string message = apiId+"|"+runImmediately;
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(exchange: "",
									 routingKey: "setup",
									 basicProperties: null,
									 body: body);

				_logger.LogInformation("[x] Sent {0} ", message);
			}
		}

		private IConnection CreateConnection(ConnectionFactory connectionFactory)
		{
			while (true)
			{
				try
				{
					_logger.LogInformation("Attempting to connect to RabbitMQ....");
					IConnection connection = connectionFactory.CreateConnection();
					return connection;
				}
				catch (BrokerUnreachableException e)
				{
					_logger.LogInformation("RabbitMQ Connection Unreachable, sleeping 5 seconds....");
					Thread.Sleep(5000);
				}
			}
		}

	}
}
