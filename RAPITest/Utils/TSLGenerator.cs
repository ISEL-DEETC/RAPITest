using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Utils
{
	public class TSLGenerator
	{
		public static List<Workflow_D> GenerateTSL(List<Workflow> workflows)
		{
			List<Workflow_D> newWorkflows = new List<Workflow_D>();

			foreach(Workflow w in workflows)
			{
				newWorkflows.Add(GenerateWorkflow(w));
			}

			return newWorkflows;
		}

		private static Workflow_D GenerateWorkflow(Workflow workflow)
		{
			Workflow_D workflow_D = new Workflow_D();
			workflow_D.WorkflowID = workflow.WorkflowID;

			workflow_D.Tests = new List<Test_D>();

			foreach (Test test in workflow.Tests)
			{
				workflow_D.Tests.Add(GenerateTest(test));
			}

			return workflow_D;
		}

		private static Test_D GenerateTest(Test test)
		{
			Test_D test_D = new Test_D();

			test_D.TestID = test.TestID;
			test_D.Server = test.Server;
			test_D.Path = test.Path;
			test_D.Method = test.Method.ToString();
			test_D.Headers = new List<string>();

			foreach(KeyValuePair<string,string> keyValuePair in test.Headers) {
				if(keyValuePair.Value != null) test_D.Headers.Add(keyValuePair.Key + ":" + keyValuePair.Value);
			}

			test_D.Body = test.Body;
			test_D.Verifications = new List<Verification_D>();

			Verification_D verification_D = new Verification_D();
			verification_D.Code = 200;

			test_D.Verifications.Add(verification_D);

			return test_D;

		}
	}
}
