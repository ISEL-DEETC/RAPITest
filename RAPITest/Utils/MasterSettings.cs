using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RAPITest.Utils
{
    public class MasterSettings
    {
        private static string masterConnectionString = null;
        private static Dictionary<string, string> masterGoogleAuth = null;
        private static Dictionary<string, string> masterFacebookAuth = null;

        public static string RetrieveConnectionString(IConfiguration configuration)
        {
            if (masterConnectionString != null)
            {
                return masterConnectionString;
            }
            // AppSettings connection string
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            // Values from environment variables
            string server = Environment.GetEnvironmentVariable("DBHOST");
            string port = Environment.GetEnvironmentVariable("DBPORT");
            string user = Environment.GetEnvironmentVariable("DBUSER");
            string password = Environment.GetEnvironmentVariable("DBPASS");

            // There is no connection string
            if (connectionString == null)
            {
                connectionString = string.Format(
                    "Server={0},{1};Database=AppDbContext;User={2};Password={3};",
                    server ?? "localhost",
                    port ?? "1433",
                    user ?? "sa",
                    password ?? "Password1234"
                );
                masterConnectionString = connectionString;
                return masterConnectionString;
            }
            // Using connection string from AppSettings
            else
            {
                SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder(connectionString);

                // If there is a server or port to mash from AppSettings
                if (server != null || port != null)
                {
                    string datasource = connBuilder.DataSource;
                    string[] datasourceParts = datasource.Split(',');
                    // Has server and port
                    if (datasourceParts.Length == 2)
                    {
                        datasource = string.Format(
                            "{0},{1}",
                            server ?? datasourceParts[0],
                            port ?? datasourceParts[1]
                        );
                    }
                    // Only has server
                    else
                    {
                        datasource = string.Format(
                            "{0},{1}",
                            server ?? datasourceParts[0],
                            port ?? "1433"
                        );
                    }

                    connBuilder.DataSource = datasource;
                }

                // Mash user
                if (user != null)
                {
                    connBuilder.UserID = user;
                }

                // Mash password
                if (password != null)
                {
                    connBuilder.Password = password;
                }
                masterConnectionString = connBuilder.ConnectionString;
                return masterConnectionString;
            }
        }

        public static Dictionary<string, string> RetrieveGoogleAuthentication(IConfiguration configuration)
        {
            if (masterGoogleAuth != null)
            {
                return masterGoogleAuth;
            }

            IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");

            string clientId = Environment.GetEnvironmentVariable("G_CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("G_CLIENT_SECRET");

            clientId = clientId ?? googleAuthNSection.GetValue<string>("ClientId");
            clientSecret = clientSecret ?? googleAuthNSection.GetValue<string>("ClientSecret");

            masterGoogleAuth = new Dictionary<string, string>();
            masterGoogleAuth["id"] = clientId;
            masterGoogleAuth["secret"] = clientSecret;
            return masterGoogleAuth;
        }

        public static Dictionary<string, string> RetrieveFacebookAuthentication(IConfiguration configuration)
        {
            if (masterFacebookAuth != null)
            {
                return masterFacebookAuth;
            }

            IConfigurationSection facebookAuthNSection = configuration.GetSection("Authentication:Facebook");

            string appId = Environment.GetEnvironmentVariable("F_APP_ID");
            string appSecret = Environment.GetEnvironmentVariable("F_APP_SECRET");

            appId = appId ?? facebookAuthNSection.GetValue<string>("AppId");
            appSecret = appSecret ?? facebookAuthNSection.GetValue<string>("AppSecret");

            masterFacebookAuth = new Dictionary<string, string>();
            masterFacebookAuth["id"] = appId;
            masterFacebookAuth["secret"] = appSecret;
            return masterFacebookAuth;
        }
    }
}

