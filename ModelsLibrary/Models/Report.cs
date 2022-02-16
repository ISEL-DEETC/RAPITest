using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
	public class Report
	{
		public Report()
		{
		}

		public Report(int errors, int warnings, DateTime date, List<Workflow> workflowResults, List<TestCombination> missingTests)
		{
			Errors = errors;
			Warnings = warnings;
			this.date = date;
			WorkflowResults = workflowResults;
			MissingTests = missingTests;
		}

		public int Errors { get; set; }
		public int Warnings { get; set; }
		public DateTime date { get; set; }
		public List<Workflow> WorkflowResults { get; set; }
		public List<TestCombination> MissingTests { get; set; }
	}
}
