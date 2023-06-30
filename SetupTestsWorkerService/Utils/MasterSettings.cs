using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SetupTestsWorkerService.Utils
{
	public class MasterSettings
	{
        private static string masterConnectionString = null;
        private static WorkerOptions masterWorkerOptions = null;

        public static string RetrieveConnectionString(WorkerOptions options)
        {
            if (masterConnectionString != null)
            {
                return masterConnectionString;
            }
            // AppSettings connection string
            string connectionString = options.DefaultConnection;

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

        public static WorkerOptions RetrieveWorkerOptions(IConfiguration configuration)
        {
            if (masterWorkerOptions != null)
            {
                return masterWorkerOptions;
            }

            WorkerOptions options = configuration.GetSection("WCF").Get<WorkerOptions>();

            options.DefaultConnection = RetrieveConnectionString(options);

            string hostname = Environment.GetEnvironmentVariable("MQHOST");
            string port = Environment.GetEnvironmentVariable("MQPORT");

            // If host is in env vars
            if (hostname != null)
            {
                options.RabbitMqHostName = hostname;
            }

            // If port is in env vars
            if (port != null)
            {
                options.RabbitMqPort = int.Parse(port);
            }

            masterWorkerOptions = options;
            return masterWorkerOptions;
        }
    }
}

