using RunTestsWorkerService.Models.AppSpecific;
using RunTestsWorkerService.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RunTestsWorkerService.Models
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

		public TestCombination Clone(TestCombination testCombination)
		{
			return new TestCombination(testCombination.Server, testCombination.Endpoint, testCombination.Method, testCombination.Consumes, testCombination.Produces, testCombination.ResponseCode);
		}

		public bool CompareTests(Test test)
		{
			if(test.Consumes != Consumes || test.Produces != Produces || test.Method != Method)
			{
				return false;
			}

			string fullPathComb = Server + Endpoint;
			string fullPathTest = test.Server + test.Path;

			if (fullPathComb != fullPathTest)
			{
				return false;
			}

			Code v = (Code)test.NativeVerifications.Find((ver) => ver.GetType() == typeof(Code));

			return ResponseCode == v.TargetCode;
		}
	}
}
