using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RAPITest.Utils
{
    public class MasterSettings
    {
        private static string masterConnectionString = null;

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
    }
}

