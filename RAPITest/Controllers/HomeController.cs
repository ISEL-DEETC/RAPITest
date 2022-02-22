using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using ModelsLibrary.Models.EFModels;
using RAPITest.Models;

namespace RAPITest.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class HomeController : Controller
	{
		private readonly RAPITestDBContext _context;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, RAPITestDBContext context, IConfiguration config)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult GetUserDetails()   //returns some statistics about the user
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
			AspNetUsers currentUser = _context.AspNetUsers.Find(userId);
			if (currentUser == null) return NoContent();
			HomeUserInfo ret = new HomeUserInfo();
			ret.NextTests = new Dictionary<string, DateTime>();
			ret.LatestReports = new Dictionary<string, ReportInfo>();
			//.Include(api => api.ExternalDll)
			List<Api> apis = _context.Api.Where(a => a.UserId == userId).ToList();

			foreach(Api api in apis)
			{
				if(api.NextTest != null)
				{
					ret.NextTests.Add(api.ApiTitle, api.NextTest.Value);
				}
				Report latestReport = _context.Report.Where(r => r.ApiId == api.ApiId).ToList().OrderByDescending(r => r.ReportDate).FirstOrDefault(); 
				if(latestReport != null)
				{
					ReportInfo reportInfo = new ReportInfo();
					reportInfo.ApiId = api.ApiId;
					reportInfo.ReportTime = latestReport.ReportDate;
					ret.LatestReports.Add(api.ApiTitle, reportInfo);
				}
			}

			List<LoginRecord> loginRecords = _context.LoginRecord.Where(l => l.UserId == userId).ToList();
			DateTime lastlogin = loginRecords.Count == 1 ? loginRecords.First().LoginTime : loginRecords[loginRecords.Count - 2].LoginTime;

			
			ret.SetupApiCount = apis.Count;
			ret.LastLogin = lastlogin;

			return Ok(ret);
		}
	}
}