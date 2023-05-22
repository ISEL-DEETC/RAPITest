using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RAPITest.Data;
using RAPITest.Models;
using Serilog;

[assembly: HostingStartup(typeof(RAPITest.Areas.Identity.IdentityHostingStartup))]
namespace RAPITest.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        private readonly ILogger _logger = Log.Logger;
        public void Configure(IWebHostBuilder builder)
        {
            try
            {
                builder.ConfigureServices((context, services) => { });
            }
            catch (Exception ex)
            {
                _logger.Error("Error during configuration, printing exception...");
                _logger.Error(ex.Message);
            }
        }
    }
}