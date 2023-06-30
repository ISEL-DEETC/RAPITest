using ModelsLibrary.Models.AppSpecific;
using RunTestsWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace RunTestsWorkerService.RunModels
{
	class RunWorkflowMultiple : RunWorkflow
	{
		private int Count;
		private int Delay;
		private Dictionary<string, List<long>> elapsedTimes;
		private readonly ILogger _logger = Log.Logger;

		public RunWorkflowMultiple(Workflow workflow, HttpUtils httpUtils, int count, int delay) : base(workflow, httpUtils)
		{
			Count = count;
			Delay = delay;
			elapsedTimes = new Dictionary<string, List<long>>();

			foreach (Test test in workflow.Tests)
			{
				elapsedTimes.Add(test.TestID, new List<long>());
			}
		}

		public new async Task<Dictionary<String, List<long>>> Run()
		{
			try
			{
				for (int i = 0; i < Count; i++)
				{
					foreach (Test test in workflow.Tests)
					{
						string ChangedPath = ChangeVariablePath(workflow, test);

						HttpRequestMessage request = httpUtils.PrepareRequestMessage(test, ChangedPath);
						Task<HttpResponseMessage> task = httpUtils.Request(request);

						var sw = Stopwatch.StartNew();
						await task;
						/*if (!task.Result.IsSuccessStatusCode)
						{
							Console.WriteLine(task.Result.StatusCode);
						}*/
						long time = sw.ElapsedMilliseconds;

						elapsedTimes.GetValueOrDefault(test.TestID).Add(time);

						Retain(workflow, test, task.Result);
					}

					Thread.Sleep(Delay);

				}

				return elapsedTimes;
			}
			catch (Exception ex)
			{
				_logger.Error($"[RunWorkflowMultiple].[Run] {ex.Message}");
				return new Dictionary<String, List<long>>();

            }
		}
	}
}
