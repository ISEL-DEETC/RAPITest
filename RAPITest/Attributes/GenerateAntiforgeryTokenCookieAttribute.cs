using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RAPITest.Attributes
{
    public class GenerateAntiforgeryTokenCookieAttribute : ResultFilterAttribute
    {
        private readonly ILogger _logger = Log.Logger;

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                var antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();

                // Send the request token as a JavaScript-readable cookie
                var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);

                context.HttpContext.Response.Cookies.Append(
                    "RequestVerificationToken",
                    tokens.RequestToken,
                    new CookieOptions() { HttpOnly = false });
            }
            catch (Exception ex)
            {
                _logger.Error("Error during OnResourceExecuting, printing exception...");
                _logger.Error(ex.Message);
            }
            
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
