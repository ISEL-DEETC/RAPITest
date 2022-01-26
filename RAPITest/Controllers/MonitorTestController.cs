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

namespace RAPITest.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class MonitorTestController : Controller
	{
		private readonly ILogger<MonitorTestController> _logger;
		private readonly string _targetFilePath;

		public MonitorTestController(ILogger<MonitorTestController> logger, IConfiguration config)
		{
			_logger = logger;
			_targetFilePath = config.GetValue<string>("TargetFilePath");
		}

		[HttpGet]
		public IActionResult GetUserAPIs() //list all user csv files
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			var userPath = Path.Combine(_targetFilePath, userId);
			List<UserInfoAPI> allAPIS = new List<UserInfoAPI>();

			string[] apis = Directory.GetDirectories(userPath);

			foreach(string dir in apis)
			{
				UserInfoAPI userRet = new UserInfoAPI();
				userRet.APITitle = dir.Split(Path.DirectorySeparatorChar).Last();

				string intervalPath = Path.Combine(dir, "NextTest.txt");
				string nextTest = "";
				using (StreamReader reader = new StreamReader(intervalPath))
				{
					nextTest = reader.ReadLine() ?? "";
				}
				if(nextTest != "")
				{
					userRet.NextTest = DateTime.Parse(nextTest);
				}

				string reportsPath = Path.Combine(dir, "Reports");
				DirectoryInfo directory = new DirectoryInfo(reportsPath);
				FileInfo newestReport = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
				if (newestReport != null)
				{
					var myJsonString = System.IO.File.ReadAllText(newestReport.FullName);
					Report report = JsonSerializer.Deserialize<Report>(myJsonString);
					userRet.Errors = report.Errors;
					userRet.Warnings = report.Warnings;
					userRet.LatestReport = newestReport.LastWriteTime;
				}

				allAPIS.Add(userRet);
			}

			return Ok(allAPIS);
		}
	}
}
