using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Models.EFModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RunTestsWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RunTestsWorkerService.RunTests
{
	public class MakeRequests
	{
		private static readonly HttpClient httpClient = new HttpClient();
		public async static Task Make(CompleteTest firstTestSetup, Api api)
		{
			int totalErrors = 0;

			foreach (Workflow workflow in firstTestSetup.Workflows)
			{
				foreach(Test test in workflow.Tests)
				{
					ChangeVariablePath(workflow, test);
					Task<HttpResponseMessage> task = Request(test);
					await task;
					totalErrors += RunVerifications(test, task.Result);
					Retain(workflow, test, task.Result);
				}
			}

			 WriteReport(firstTestSetup,totalErrors, api);
		}

		private static void ChangeVariablePath(Workflow workflow, Test test)
		{
			List<string> foundVarPath = test.GetVariablePathKeys();
			foreach(string var in foundVarPath)
			{
				test.Path = test.Path.Replace("{" + var + "}", workflow.Retain.GetValueOrDefault(var).Value);
			}
		}

		private async static void Retain(Workflow workflow, Test test, HttpResponseMessage response)
		{
			if (test.Retain == null) return;
			foreach (string key in test.Retain)
			{
				Retained retained = workflow.Retain.GetValueOrDefault(key.Split('#')[0]);
				string body = await response.Content.ReadAsStringAsync();

				//support json and xml but for now only support json
				JObject obj = JObject.Parse(body);
				JToken val = obj.SelectToken(retained.Path);
				if (val == null)
				{
					retained.Value = "";
				}
				else
				{
					retained.Value = val.ToString();
				}
			}
		}

		private static int RunVerifications(Test test, HttpResponseMessage response)
		{
			int totalErrors = 0;
			if (test.TestResults == null) test.TestResults = new List<Result>();
			foreach (Verification verification in test.NativeVerifications)
			{
				Result r = new Result();
				r = verification.Verify(response);
				if (!r.Success) totalErrors++;
				test.TestResults.Add(r);
			}
			foreach (dynamic verification in test.ExternalVerifications)
			{
				Result r = new Result();
				dynamic result = verification.Verify(response);
				r.Success = result.Success;
				r.Description = result.Description;
				r.TestName = result.TestName;
				if (!r.Success) totalErrors++;
				test.TestResults.Add(r);
			}
			return totalErrors;
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

		public static void WriteReport(CompleteTest firstTestSetup, int totalErrors, Api api)
		{
			ModelsLibrary.Models.Report report = new ModelsLibrary.Models.Report();
			report.Errors = totalErrors;
			report.WorkflowResults = firstTestSetup.Workflows;
			report.Warnings = firstTestSetup.MissingTests.Count();
			report.MissingTests = firstTestSetup.MissingTests;
			report.date = DateTime.Now;

			ModelsLibrary.Models.EFModels.Report r = new ModelsLibrary.Models.EFModels.Report();
			r.ReportFile = Encoding.Default.GetBytes(JsonSerialization.SerializeToJsonModed(report));
			r.ReportDate = report.date;
			r.ApiId = api.ApiId;

			api.Report.Add(r);
		}
	}
}
