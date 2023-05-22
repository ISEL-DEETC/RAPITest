using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace RAPITest.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ErrorModel : PageModel
	{
		private readonly ILogger _logger;

		public ErrorModel(ILogger logger)
		{
			_logger = logger;
		}

		public string RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

		public void OnGet()
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
		}
	}
}
