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

namespace RAPITest.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class MonitorTestController : Controller
	{
		private readonly ILogger<MonitorTestController> _logger;
		private readonly RAPITestDBContext _context;

		public MonitorTestController(ILogger<MonitorTestController> logger, RAPITestDBContext context, IConfiguration config)
		{
			_logger = logger;
			_context = context;
		}

		[HttpGet]
		public IActionResult GetUserAPIs() 
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

			List<UserInfoAPI> allAPIS = new List<UserInfoAPI>();

			List<Api> apis = _context.Api.Where(a => a.UserId == userId).ToList();
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
	}
}
