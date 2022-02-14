using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using SetupTestsWorkerService.SetupTests;
using System.Timers;
using ModelsLibrary.Models.EFModels;

namespace SetupTestsWorkerService
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly WorkerOptions options;

		public Worker(ILogger<Worker> logger, WorkerOptions options)
		{
			_logger = logger;
			this.options = options;
            StartupSetupTimers();
        }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "setup",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    _logger.LogInformation("Recieved {0}", message);
                    ThreadPool.QueueUserWorkItem((state) => Work(message));
                };
                channel.BasicConsume(queue: "setup",
                                     autoAck: true,
                                     consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(10000, stoppingToken);
                }
            }
        }

        private void Work(string message)
        {
            int apiId = int.Parse(message.Split("|")[0]);
            bool runNow = bool.Parse(message.Split("|")[1]);

			if (runNow)
			{
                RunNow(apiId);
			}
			else
			{
                RAPITestDBContext _context;
                var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
                optionsBuilder.UseSqlServer(options.DefaultConnection);

                using (_context = new RAPITestDBContext(optionsBuilder.Options))
                {
                    Api api = _context.Api.Find(apiId);
                    SetupSpecificTimer(api);
                }
            }
        }

        public void StartupSetupTimers()
		{
            RAPITestDBContext _context;
            var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
            optionsBuilder.UseSqlServer(options.DefaultConnection);

            using (_context = new RAPITestDBContext(optionsBuilder.Options))
            {
                List<Api> apis = _context.Api.Where(api => api.NextTest != null).ToList();

                foreach(Api api in apis)
				{
					SetupSpecificTimer(api);
                }
				
            }
        }

        public void SetupSpecificTimer(Api api)
		{
            double timer = (api.NextTest - DateTime.Now).Value.TotalMilliseconds;
			if (timer < 0)
			{
                timer = 1000;
            }
            System.Timers.Timer aTimer = new System.Timers.Timer(timer);
            aTimer.Elapsed += (sender, e) => RunCallback(sender, e, api.ApiId);
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        public void RunCallback(object sender, ElapsedEventArgs e, int apiId)
		{
            RAPITestDBContext _context;
            var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
            optionsBuilder.UseSqlServer(options.DefaultConnection);

            using (_context = new RAPITestDBContext(optionsBuilder.Options))
            {
                if (SetupTestRun.Run(apiId, _context))
                {
                    Sender(apiId);
                    Api api = _context.Api.Find(apiId);
                    if (api.TestTimeLoop.HasValue)
                    {
                        SetupNextTest(api, _context);
                    }
                }
            }

            _logger.LogInformation("Api {0} - Work Complete", apiId);
        }

        public void RunNow(int apiId)
        {
            RAPITestDBContext _context;
            var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
            optionsBuilder.UseSqlServer(options.DefaultConnection);

            using (_context = new RAPITestDBContext(optionsBuilder.Options))
            {
                if (SetupTestRun.Run(apiId, _context))
                {
                    Sender(apiId);
                    Api api = _context.Api.Find(apiId);
                    if (api.TestTimeLoop.HasValue) 
                    {
                        SetupNextTest(api, _context);
                    }
                }
            }

            _logger.LogInformation("Api {0} - Work Complete", apiId);
        }

        public void SetupNextTest(Api api, RAPITestDBContext _context)
		{
            //hours to seconds (3600) to milliseconds (1000)
            DateTime nextTest = DateTime.Now.AddHours(api.TestTimeLoop.Value);
            System.Timers.Timer aTimer = new System.Timers.Timer((nextTest - DateTime.Now).TotalMilliseconds);
            aTimer.Elapsed += (sender, e) => RunCallback(sender, e, api.ApiId);
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
            api.NextTest = nextTest;
            _context.SaveChanges();
		}

        public void Sender(int apiId)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };   //as longs as it is running in the same machine
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "run",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = apiId + "";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "run",
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation("[x] Sent {0} ", message);
            }
        }
    }
}
