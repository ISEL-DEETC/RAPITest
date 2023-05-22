using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
using RAPITest.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RAPITest
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetService<ILogger>();

            ApplyMigrations(services, logger);

            await host.RunAsync();
        }

        public static void ApplyMigrations(IServiceProvider services, ILogger logger)
        {
            while (true)
            {
                try
                {
                    logger.Information("Attempting to connect to Database....");
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    if (dbContext.Database.IsSqlServer())
                    {
                        dbContext.Database.Migrate();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warning("Database Unreachable, sleeping 5 seconds....");
                    logger.Error(ex.Message);
                    Thread.Sleep(5000);
                }
            }

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
                .ConfigureWebHostDefaults(webBuilder =>

                {
                    webBuilder.UseStartup<Startup>();
                });
            }
            catch (Exception ex)
            {
                //logger.Error("Creating host builder, printing exception...");
                logger.Error(ex.Message);
                return null;
            }
        }
	}
}
