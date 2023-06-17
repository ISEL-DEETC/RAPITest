using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelsLibrary.Models.EFModels;
using RAPITest.Data;
using RAPITest.Models;
using RAPITest.Utils;
using Microsoft.AspNetCore.Http.Features;
using System;
using Microsoft.VisualBasic;
using Serilog;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace RAPITest
{
	public class Startup
	{
		private readonly ILogger _logger = Log.ForContext<Startup>();

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			try
			{
                var connectionString = MasterSettings.RetrieveConnectionString(Configuration);

                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

                services.AddDbContext<RAPITestDBContext>(options =>
                    options.UseSqlServer(connectionString));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<ApplicationDbContext>();

                services.AddIdentityServer()
                    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

                services.AddAuthentication()
                    .AddIdentityServerJwt();

                services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    });

                services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        Dictionary<string, string> googleAuth = MasterSettings.RetrieveGoogleAuthentication(Configuration);

                        options.ClientId = googleAuth["id"];
                        options.ClientSecret = googleAuth["secret"];
                    })
                    .AddFacebook(options =>
                    {
                        Dictionary<string, string> facebookAuth = MasterSettings.RetrieveFacebookAuthentication(Configuration);

                        options.AppId = facebookAuth["id"];
                        options.AppSecret = facebookAuth["secret"];
                        options.AccessDeniedPath = "/AccessDeniedPathInfo";
                    });

                services.AddControllersWithViews();
                services.AddRazorPages();

                // In production, the React files will be served from this directory
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/build";
                });

                services.AddRazorPages();

                // In production, the React files will be served from this directory
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/build";
                });
            }
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			try
			{
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseSpaStaticFiles();

                app.UseRouting();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseAuthentication();
                app.UseIdentityServer();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });

                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            }
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
			}
		}
	}
}
