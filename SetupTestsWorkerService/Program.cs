using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace SetupTestsWorkerService
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
                
            return Host.CreateDefaultBuilder(args)
                .UseSerilog(logger)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    WorkerOptions options = configuration.GetSection("WCF").Get<WorkerOptions>();

                    services.AddSingleton(options);

                    services.AddHostedService<Worker>();
                });
        }
			
	}
}
