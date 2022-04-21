using ModelsLibrary.Models.Deserialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
	public class Workflow_D
	{
		public string WorkflowID { get; set; }

		public Stress_D Stress { get; set; }

		public List<Test_D> Tests { get; set; }
	}
}
