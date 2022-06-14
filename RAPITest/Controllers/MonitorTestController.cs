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
using RAPITest.Models;
using System;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models.EFModels;
using Newtonsoft.Json;
using ModelsLibrary.Models.AppSpecific;
using RAPITest.Utils;
using RabbitMQ.Client;

namespace RAPITest.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class MonitorTestController : Controller
	{
		private readonly ILogger<MonitorTestController> _logger;
		private readonly RAPITestDBContext _context;
		private readonly string RabbitMqHostName;
		private readonly int RabbitMqPort;

		public MonitorTestController(ILogger<MonitorTestController> logger, RAPITestDBContext context, IConfiguration config)
		{
			_logger = logger;
			_context = context;
			RabbitMqHostName = config.GetValue<string>("RabbitMqHostName");
			RabbitMqPort = config.GetValue<int>("RabbitMqPort");
		}

		[HttpGet]
		public IActionResult GetUserAPIs() 
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

			List<UserInfoAPI> allAPIS = new List<UserInfoAPI>();

			List<Api> apis = _context.Api.Where(a => a.UserId == userId).ToList();
			apis = apis.FindAll(api => !(Encoding.Default.GetString(api.Tsl).Equals("") && !api.RunGenerated));

			foreach(Api api in apis)
			{
				UserInfoAPI userInfoAPI = new UserInfoAPI();
				userInfoAPI.APITitle = api.ApiTitle;
				userInfoAPI.NextTest = api.NextTest.GetValueOrDefault();
				userInfoAPI.ApiId = api.ApiId;

				ModelsLibrary.Models.EFModels.Report report = _context.Report.Where(r => r.ApiId == api.ApiId).OrderByDescending(r => r.ReportDate).FirstOrDefault();
				if(report != null)
				{
					string text = Encoding.Default.GetString(report.ReportFile);
					if(text[0] == '{')
					{
						//valid report
						ModelsLibrary.Models.Report re = JsonConvert.DeserializeObject<ModelsLibrary.Models.Report>(text);
						userInfoAPI.Errors = re.Errors;
						userInfoAPI.Warnings = re.Warnings;
						userInfoAPI.LatestReport = report.ReportDate;
					}
					else
					{
						//error report
						userInfoAPI.ErrorMessages = text.Split('\n').ToList();
					}
				}

				allAPIS.Add(userInfoAPI);
			}

			return Ok(allAPIS);
		}
		
		[HttpGet]
		public IActionResult DownloadReport([FromQuery] int apiId)   
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

			ModelsLibrary.Models.EFModels.Report report = _context.Report.Where(r => r.ApiId == apiId).OrderByDescending(r => r.ReportDate).FirstOrDefault();
			if (report == null) return NotFound();

			string rep = Encoding.Default.GetString(report.ReportFile);
			return Ok(rep);
		}

		[HttpPut]
		public IActionResult ChangeApiTitle([FromQuery] int apiId, [FromQuery] string newTitle)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			using (_context)
			{
				ModelsLibrary.Models.EFModels.Api api = _context.Api.Where(a => a.ApiId == apiId).FirstOrDefault();
				if (api == null) return NotFound();
				api.ApiTitle = newTitle;
				_context.SaveChanges();
			}
			return Ok();
		}

		[HttpGet]
		public IActionResult ReturnReport([FromQuery] int apiId) 
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

			IOrderedQueryable<ModelsLibrary.Models.EFModels.Report> reports = _context.Report.Include(report => report.Api).Where(r => r.ApiId == apiId).OrderByDescending(r => r.ReportDate);
			ModelsLibrary.Models.EFModels.Report report = reports.FirstOrDefault();
			if (report == null) return NotFound();

			VisualizeReportModel v = new VisualizeReportModel();
			v.Report = Encoding.Default.GetString(report.ReportFile);
			v.ApiName = report.Api.ApiTitle;

			List<DateTime> dateTimes = new List<DateTime>();

			reports.AsEnumerable().ToList().ForEach(x => dateTimes.Add(x.ReportDate));

			v.AllReportDates = dateTimes;

			return Ok(v);
		}

		[HttpGet]
		public IActionResult ReturnReportSpecific([FromQuery] int apiId, DateTime date)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

			IOrderedQueryable<ModelsLibrary.Models.EFModels.Report> reports = _context.Report.Include(report => report.Api).Where(r => r.ApiId == apiId).OrderByDescending(r => r.ReportDate);
			ModelsLibrary.Models.EFModels.Report report = reports.Where(x => x.ReportDate.Date == date.Date && x.ReportDate.Hour == date.Hour && x.ReportDate.Minute == date.Minute && x.ReportDate.Second == date.Second).FirstOrDefault();
			if (report == null) return NotFound();

			VisualizeReportModel v = new VisualizeReportModel();
			v.Report = Encoding.Default.GetString(report.ReportFile);
			v.ApiName = report.Api.ApiTitle;

			List<DateTime> dateTimes = new List<DateTime>();

			reports.AsEnumerable().ToList().ForEach(x => dateTimes.Add(x.ReportDate));

			v.AllReportDates = dateTimes;

			return Ok(v);
		}

		[HttpGet]
		public IActionResult GenerateMissingTestsTSL([FromQuery] int apiId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

			IOrderedQueryable<ModelsLibrary.Models.EFModels.Report> reports = _context.Report.Include(report => report.Api).Where(r => r.ApiId == apiId).OrderByDescending(r => r.ReportDate);
			ModelsLibrary.Models.EFModels.Report report = reports.FirstOrDefault();
			if (report == null) return NotFound();

			ModelsLibrary.Models.Report rep = Newtonsoft.Json.JsonConvert.DeserializeObject<ModelsLibrary.Models.Report>(Encoding.Default.GetString(report.ReportFile));

			List<Workflow> workflows = new List<Workflow>();
			Workflow w = new Workflow();
			w.Tests = rep.MissingTests;
			w.WorkflowID = "MissingTestsTSL";
			
			workflows.Add(w);

			List<ModelsLibrary.Models.Workflow_D> file = TSLGenerator.GenerateTSL(workflows);

			return Ok(file);
		}

		[HttpDelete]
		public IActionResult RemoveApi([FromQuery] int apiId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			Api api = _context.Api.Include(api => api.ExternalDll).Include(api => api.Report).Where(a => a.UserId == userId && a.ApiId == apiId).FirstOrDefault();
			if (api == null) return NotFound();

			_context.Api.Remove(api);
			_context.SaveChanges();
			return Ok();
		}

		[HttpGet]
		public IActionResult RunNow([FromQuery] int apiId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			Api api = _context.Api.Include(api => api.ExternalDll).Include(api => api.Report).Where(a => a.UserId == userId && a.ApiId == apiId).FirstOrDefault();
			if (api == null) return NotFound();

			Sender(apiId);
			return Ok();
		}

		public void Sender(int apiId)
		{
			var factory = new ConnectionFactory() { HostName = RabbitMqHostName, Port = RabbitMqPort };   //as longs as it is running in the same machine
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(queue: "run",
									 durable: false,
									 exclusive: false,
									 autoDelete: false,
									 arguments: null);

				string message = apiId + "";
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(exchange: "",
									 routingKey: "run",
									 basicProperties: null,
									 body: body);

				_logger.LogInformation("[x] Sent {0} ", message);
			}
		}
	}
}
