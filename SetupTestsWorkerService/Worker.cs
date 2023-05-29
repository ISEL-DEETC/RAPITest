using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using SetupTestsWorkerService.SetupTests;
using System.Timers;
using ModelsLibrary.Models.EFModels;
using RabbitMQ.Client.Exceptions;
using Microsoft.Data.SqlClient;
using Serilog;

namespace SetupTestsWorkerService
{
	public class Worker : BackgroundService
	{
		private readonly ILogger _logger;
		private readonly WorkerOptions options;

        private static readonly string QUEUE_NAME = "setup";

		public Worker(ILogger logger, WorkerOptions options)
		{
			_logger = logger;
			this.options = options;
            StartupSetupTimers();
        }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
            try
            {

                var factory = new ConnectionFactory() { HostName = this.options.RabbitMqHostName, Port = this.options.RabbitMqPort };
                using (var connection = CreateConnection(factory))
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        _logger.Information("Received {0}", message);
                        ThreadPool.QueueUserWorkItem((state) => Work(channel, ea, message));
                    };
                    channel.BasicConsume(
                        queue: QUEUE_NAME,
                        autoAck: false,
                        consumer: consumer
                    );

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        await Task.Delay(10000, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                await Task.FromException(ex);
            }
        }

        private IConnection CreateConnection(ConnectionFactory connectionFactory)
        {
            while (true)
            {
                try
                {
                    _logger.Information("Attempting to connect to RabbitMQ....");
                    IConnection connection = connectionFactory.CreateConnection();
                    return connection;
                }
                catch (BrokerUnreachableException)
                {
                    _logger.Information("RabbitMQ Connection Unreachable, sleeping 5 seconds....");
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
        }

        private void Work(IModel channel, BasicDeliverEventArgs ea, string message)
        {
            try
            {

                string[] args = message.Split("|");

                int apiId = int.Parse(args[0]);
                bool runNow = bool.Parse(args[1]);

                RAPITestDBContext _context;
                var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
                optionsBuilder.UseSqlServer(options.DefaultConnection);

                using (_context = new RAPITestDBContext(optionsBuilder.Options))
                {
                    if (SetupTestRun.Run(apiId, _context))
                    {
                        if (runNow)
                        {
                            Sender(apiId);
                            Api api = _context.Api.Find(apiId);
                            if (api.TestTimeLoop.HasValue)
                            {
                                SetupNextTest(api, _context);
                            }
                        }
                        else
                        {
                            _logger.Information("Api {0} - Work Complete", apiId);
                        }
                    }
                    else
                    {
                        _logger.Information("Api {0} - Work Complete", apiId);
                    }
                }
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        }

        public void StartupSetupTimers()
		{
            RAPITestDBContext _context;
            var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
            optionsBuilder.UseSqlServer(options.DefaultConnection);

            using (_context = new RAPITestDBContext(optionsBuilder.Options))
            {
                DatabaseConnection(_context);
                List<Api> apis = _context.Api.Where(api => api.NextTest != null).ToList();

                foreach(Api api in apis)
				{
					SetupSpecificTimer(api);
                }
				
            }
        }

		private void DatabaseConnection(RAPITestDBContext context)
		{
            while (true)
            {
                try
                {
                    _logger.Information("Attempting to connect to Database....");
                    context.Database.CanConnect();
                    List<Api> apis = context.Api.Where(api => api.NextTest != null).ToList();
                    return;
                }
                catch (SqlException)
                {
                    _logger.Information("Database Unreachable, sleeping 5 seconds....");
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
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

            _logger.Information("Api {0} - Work Complete", apiId);
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
            var factory = new ConnectionFactory() { HostName = this.options.RabbitMqHostName, Port = this.options.RabbitMqPort };   //as longs as it is running in the same machine
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

                _logger.Information("[x] Sent {0} ", message);
            }
        }
    }
}
