using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RunTestsWorkerService.Utils;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using Serilog;
using ModelsLibrary.Models.Language;

namespace RunTestsWorkerService.RunModels
{
	class RunWorkflow
	{
		protected Workflow workflow;
		protected HttpUtils httpUtils;

		public RunWorkflow(Workflow workflow, HttpUtils httpUtils)
		{
			this.workflow = workflow;
			this.httpUtils = httpUtils;
		}

		public async Task Run()
		{
			try
			{
				foreach (Test test in workflow.Tests)
				{
					string ChangedPath = ChangeVariablePath(workflow, test);

					HttpRequestMessage request = httpUtils.PrepareRequestMessage(test, ChangedPath);
					Task<HttpResponseMessage> task = httpUtils.Request(request);
					var sw = Stopwatch.StartNew();
					await task;
					long time = sw.ElapsedMilliseconds;
					httpUtils.RunVerifications(test, task.Result);

					httpUtils.FillRequestMetadata(test, request, task.Result, time);

					Retain(workflow, test, task.Result);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex.Message);
			}
		}

		protected string ChangeVariablePath(Workflow workflow, Test test)
		{
			try
			{
				string auxPath = test.Path;
				List<string> foundVarPath = test.GetVariablePathKeys();
				foreach (string var in foundVarPath)
				{
					auxPath = test.Path.Replace("{" + var + "}", workflow.Retain.GetValueOrDefault(var).Value);
				}
				return auxPath;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex.Message);
				return "Error occurred";
			}
		}

		protected async void Retain(Workflow workflow, Test test, HttpResponseMessage response)
		{
			try
			{
				if (test.Retain == null) return;
				foreach (string key in test.Retain)
				{
					Retained retained = workflow.Retain.GetValueOrDefault(key.Split('#')[0]);
					string body = await response.Content.ReadAsStringAsync();

					ALanguage language = ALanguage.GetLanguage(retained.Path);
					retained.Value = language.GetValue(retained.Path, body);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex.Message);
			}
		}

		
	}
}
