using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RAPITest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            ApplyMigrations(services, logger);

            await host.RunAsync();
        }

        public static void ApplyMigrations(IServiceProvider services, ILogger logger)
		{
			while (true)
			{
                try
                {
                    logger.LogInformation("Attempting to connect to Database....");
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    if (dbContext.Database.IsSqlServer())
                    {
                        dbContext.Database.Migrate();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogInformation("Database Unreachable, sleeping 5 seconds....");
                }
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
