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
			ret.LatestActions = new List<ApiInfo>();
			List<Api> apis = _context.Api.Where(a => a.UserId == userId).ToList();

			foreach(Api api in apis)
			{
				ApiInfo apiInfo = new ApiInfo();
				apiInfo.ApiId = api.ApiId;
				apiInfo.Title = api.ApiTitle;
				if(api.NextTest != null)
				{
					apiInfo.NextTest = api.NextTest.Value;
				}
				Report latestReport = _context.Report.Where(r => r.ApiId == api.ApiId).ToList().OrderByDescending(r => r.ReportDate).FirstOrDefault(); 
				if(latestReport != null)
				{
					apiInfo.ReportDate = latestReport.ReportDate;
				}
				ret.LatestActions.Add(apiInfo);
			}

			List<LoginRecord> loginRecords = _context.LoginRecord.Where(l => l.UserId == userId).ToList();
			DateTime lastlogin = loginRecords.Count == 1 ? loginRecords.First().LoginTime : loginRecords[loginRecords.Count - 2].LoginTime;

			ret.SetupApiCount = apis.Count;
			ret.LastLogin = lastlogin;

			return Ok(ret);
		}
	}
}