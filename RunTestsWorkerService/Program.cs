using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
using System.Threading;
using RunTestsWorkerService.Utils;

namespace RunTestsWorkerService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //Initialize Logger
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

			Log.Logger = logger;

            try
			{

				return Host.CreateDefaultBuilder(args)
					.UseSerilog(logger)
					.ConfigureServices((hostContext, services) =>
					{
                        IConfiguration configuration = hostContext.Configuration;

						WorkerOptions options = MasterSettings.RetrieveWorkerOptions(configuration);

						services.AddSingleton(options);

						services.AddHostedService<Worker>();
					});
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				return null;
			}
		}
	}
}
