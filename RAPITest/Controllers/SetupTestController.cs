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
using RAPITest.RunTests;

namespace DataAnnotation.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	//[GenerateAntiforgeryTokenCookie]
	public class SetupTestController : Controller
	{
		private readonly ILogger<SetupTestController> _logger;
		private readonly string _targetFilePath;
		private readonly long _fileSizeLimit;
		private readonly string[] _permittedExtensions;

		private static readonly HttpClient _httpClient;
		private static readonly FormOptions _defaultFormOptions;

		static SetupTestController()
		{
			_httpClient = new HttpClient();
			_defaultFormOptions = new FormOptions();
		}
		public SetupTestController(ILogger<SetupTestController> logger, IConfiguration config)
		{
			_logger = logger;
			_targetFilePath = config.GetValue<string>("TargetFilePath");
			_fileSizeLimit = config.GetValue<long>("FileSizeLimit");
			_permittedExtensions = config.GetSection("PermittedExtensionsApiSpecification").GetChildren().ToArray().Select(v => v.Value).ToArray();
		}

		[HttpPost]
		[DisableRequestSizeLimit]
		[DisableFormValueModelBinding]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadFile(IFormCollection data)
		{
			List<IFormFile> files = data.Files.ToList();
			
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			var userPath = Path.Combine(_targetFilePath, userId);
			Directory.CreateDirectory(userPath);
			var testDirectory = Path.Combine(userPath, data["name"]);

			if (Directory.Exists(testDirectory))
			{
				return BadRequest("This API is already being tested! Give a different title or delete the old one first.");
			}

			Directory.CreateDirectory(testDirectory);

			var pathReports = Path.Combine(testDirectory, "Reports");
			Directory.CreateDirectory(pathReports);
			var pathTestInformation = Path.Combine(testDirectory, "TestInformation");
			Directory.CreateDirectory(pathTestInformation);
			var pathDLL = Path.Combine(pathTestInformation, "DLLs");
			Directory.CreateDirectory(pathDLL);
			var pathDictionary = Path.Combine(pathTestInformation, "Dictionary");
			Directory.CreateDirectory(pathDictionary);

			foreach (var formFile in files)
			{
				if (formFile.Length > 0)
				{
					var path = "";
					if (formFile.Name.Contains("tsl_"))
					{
						path = Path.Combine(pathTestInformation, formFile.Name);
					}
					else if(formFile.Name.Contains("apiSpecification"))
					{
						path = Path.Combine(testDirectory, formFile.Name);
					}
					else if(formFile.Name.Contains("dictionary"))
					{
						path = Path.Combine(pathDictionary, formFile.Name);
					}
					else
					{
						path = Path.Combine(pathDLL, formFile.Name);
					}
					using (var stream = System.IO.File.Create(path))
					{
						await formFile.CopyToAsync(stream);
					}
				}
			}

			var pathInterval = Path.Combine(testDirectory, "NextTest.txt");
			using (StreamWriter outputFile = new StreamWriter(pathInterval))
			{
				string nextTest = "";
				string nextInterval = "";
				//radioButtons: [button1H, button12H, button24H, button1W, button1M, buttonNever] 
				switch (data["interval"])
				{
					case "1 hour":
						nextTest = DateTime.Now.AddHours(1).ToString();
						nextInterval = "1 hour";
						break;
					case "12 hours":
						nextTest = DateTime.Now.AddHours(12).ToString();
						nextInterval = "12 hours";
						break;
					case "24 hours":
						nextTest = DateTime.Now.AddDays(1).ToString();
						nextInterval = "24 hours";
						break;
					case "1 week":
						nextTest = DateTime.Now.AddDays(7).ToString();
						nextInterval = "1 week";
						break;
					case "1 month":
						nextTest = DateTime.Now.AddMonths(1).ToString();
						nextInterval = "1 month";
						break;
					default:  //Never
						break;
				}
				outputFile.WriteLine(nextTest);
				outputFile.WriteLine(nextInterval);
			}

			if (data["runimmediately"] == "true")
			{
				//SetupTestRun.Run(testDirectory);
			}
			return Created(nameof(SetupTestController), null);
		}

	}
}
