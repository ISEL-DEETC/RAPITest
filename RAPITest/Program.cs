using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RAPITest.SetupTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			SetupTestRun.Run("D:\\isel\\2022INVERNO\\TESE\\APPUsers\\58cf744f-4daf-42a3-ba80-6b5b9bc7b438\\Amazon");
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
