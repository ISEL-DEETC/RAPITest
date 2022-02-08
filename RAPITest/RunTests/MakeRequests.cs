using RAPITest.Models;
using RAPITest.Models.AppSpecific;
using RAPITest.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RAPITest.RunTests
{
	public class MakeRequests
	{
		private static readonly HttpClient httpClient = new HttpClient();
		public async static void Make(FirstTestSetup firstTestSetup)
		{
			Dictionary<Test,Task<HttpResponseMessage>> resultsTasks = new Dictionary<Test,Task<HttpResponseMessage>>();

			foreach (Workflow workflow in firstTestSetup.Workflows)
			{
				foreach(Test test in workflow.Tests)
				{
					Task<HttpResponseMessage> task = Request(test);
					resultsTasks.Add(test, task);
				}
			}

			await Task.WhenAll(resultsTasks.Values.ToList());

			int totalErrors = 0;

			foreach (KeyValuePair<Test, Task<HttpResponseMessage>> entry in resultsTasks)
			{
				HttpResponseMessage response = await entry.Value;
				if (entry.Key.TestResults == null) entry.Key.TestResults = new List<Result>();
				foreach (Verification verification in entry.Key.NativeVerifications)
				{
					Result r = new Result();
					r = verification.Verify(response);
					if (!r.Success) totalErrors++;
					entry.Key.TestResults.Add(r);
				}
				foreach (dynamic verification in entry.Key.ExternalVerifications)
				{
					Result r = new Result();
					dynamic result = verification.Verify(response);
					r.Success = result.Success;
					r.Description = result.Description;
					r.TestName = result.TestName;
					if (!r.Success) totalErrors++;
					entry.Key.TestResults.Add(r);
				}
				
				
			}

			WriteReport(firstTestSetup,totalErrors);
		}

		public async static Task<HttpResponseMessage> Request(Test test)
		{
			using (var requestMessage = new HttpRequestMessage(Convert(test.Method), test.Server+test.Path))
			{

				requestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(test.Produces));
				if (test.Body != null)
				{
					requestMessage.Content = new StringContent(test.Body, Encoding.UTF8, test.Consumes);
				}

				return await httpClient.SendAsync(requestMessage);
			}
		}

		public static HttpMethod Convert(Method method)
		{
			switch (method)
			{
				case (Method.Get):
					return HttpMethod.Get;
				case (Method.Post):
					return HttpMethod.Post;
				case (Method.Put):
					return HttpMethod.Put;
				case (Method.Delete):
					return HttpMethod.Delete;
			}
			return null;
		}

		public static void WriteReport(FirstTestSetup firstTestSetup, int totalErrors)
		{
			Report report = new Report();
			report.Errors = totalErrors;
			report.WorkflowResults = firstTestSetup.Workflows;
			report.Warnings = firstTestSetup.MissingTests.Count();
			report.MissingTests = firstTestSetup.MissingTests;
			report.date = DateTime.Now;

			string reportsPath = Path.Combine(firstTestSetup.ApiPath, "Reports");
			JsonSerialization.WriteToJsonFileModed<Report>(Path.Combine(reportsPath, "report_"+ DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") +".json"), report);
		}
	}
}
