using ModelsLibrary.Models.AppSpecific;
using RunTestsWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RunTestsWorkerService.RunModels
{
	class StressTests
	{
		private Workflow workflow;
		private HttpUtils httpUtils;
		private int ThreadNumber;
		private int Count;
		private int Delay; 
		

		public StressTests(Workflow workflow, HttpUtils httpUtils,int ThreadNumber, int Count, int Delay)
		{
			this.workflow = workflow;
			this.httpUtils = httpUtils;
			this.Count = Count;
			this.ThreadNumber = ThreadNumber;
			this.Delay = Delay;
		}

		public async Task<Dictionary<String, List<long>>> Run()
		{
			Dictionary<String, List<long>> ret = new Dictionary<string, List<long>>();

			List<Task> tasks = new List<Task>();

			int iterationsPerThread = Count / ThreadNumber;

			for(int i = 0; i < ThreadNumber; i++)
			{
				RunWorkflowMultiple twf = new RunWorkflowMultiple(workflow, httpUtils,iterationsPerThread,Delay);
				tasks.Add(twf.Run().ContinueWith(dic =>{
					Merge(ret, dic.Result);
				}));
			}

			await Task.WhenAll(tasks);

			return ret;
		}

		public void Merge(Dictionary<String, List<long>> me, Dictionary<String, List<long>> merge)
		{
			foreach (var item in merge)
			{
				if (!me.ContainsKey(item.Key))
				{
					me[item.Key] = item.Value;
				}
				else
				{
					me[item.Key].AddRange(item.Value);
				}
			}
		}

	}
}
