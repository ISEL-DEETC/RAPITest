using System;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Serilog;
//using Microsoft.Extensions.Logging;

namespace RAPITest.Controllers
{
	public class OidcConfigurationController : Controller
	{
		private readonly ILogger _logger;

		public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider, ILogger logger)
		{
			ClientRequestParametersProvider = clientRequestParametersProvider;
			_logger = logger;
		}

		public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

		[HttpGet("_configuration/{clientId}")]
		public IActionResult GetClientRequestParameters([FromRoute] string clientId)
		{
			try
			{
                var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
                return Ok(parameters);
            }
			catch (Exception ex)
			{
				_logger.Error("Error in GetClientRequestParameters, printing exception...");
				_logger.Error(ex.Message);
				return Problem(ex.Message);
			}
			
		}
	}
}
