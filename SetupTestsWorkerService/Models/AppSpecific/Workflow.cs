using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetupTestsWorkerService.Models.AppSpecific
{
	[Serializable]
	public class Workflow
	{
		public string WorkflowID { get; set; }

		public List<Test> Tests { get; set; }
	}
}
