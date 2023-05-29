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
using RunTestsWorkerService.RunTests;
using ModelsLibrary.Models.EFModels;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace RunTestsWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly WorkerOptions options;

        private static readonly string QUEUE_NAME = "run";

        public Worker(ILogger logger, WorkerOptions options)
        {
            _logger = logger;
            this.options = options;
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
                        _logger.Information("Recieved {0}", message);
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
                catch(BrokerUnreachableException)
				{
                    _logger.Warning("RabbitMQ Connection Unreachable, sleeping 5 seconds....");
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
			}
		}


        private async void Work(IModel channel, BasicDeliverEventArgs ea, string message)
        {
            try
            {
                int apiId = Int32.Parse(message);
                await RunApiTests.RunAsync(apiId, options.DefaultConnection);
                _logger.Information("Message {0} - Work Complete", message);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        }
    }
}

