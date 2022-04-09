using System;
using System.Collections.Generic;
using System.Text;

namespace RunTestsWorkerService
{
	public class WorkerOptions
	{
		public string DefaultConnection { get; set; }
		public string RabbitMqHostName { get; set; }
		public int RabbitMqPort { get; set; }
	}
}
