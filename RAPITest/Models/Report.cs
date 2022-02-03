using RAPITest.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models
{
	public class Report
	{
		public int Errors { get; set; }
		public int Warnings { get; set; }
		public List<Workflow> WorkflowResults { get; set; }
		public List<TestCombination> MissingTests { get; set; }
	}
}
