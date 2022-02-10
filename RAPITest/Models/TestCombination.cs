using RAPITest.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models
{
	//step 7 - Check if any combination of server/endpoint/input/output/code isn't beeing tested

	public class TestCombination
	{
		public TestCombination(string server, string endpoint, Method method, string consumes, string produces, int responseCode)
		{
			Server = server;
			Endpoint = endpoint;
			Method = method;
			Consumes = consumes;
			Produces = produces;
			ResponseCode = responseCode;
		}

		public TestCombination() { }

		public string Server { get; set; }

		public string Endpoint { get; set; }

		public Method Method { get; set; }

		public string Consumes { get; set; }

		public string Produces { get; set; }

		public int ResponseCode { get; set; }
	}
}
