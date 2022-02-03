using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RAPITest.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models
{
	public class FirstTestSetup
	{
		public string ApiPath { get; set; }

		[JsonIgnore]
		public OpenApiDocument ApiSpecification { get; set; }

		[JsonIgnore]
		public List<Workflow_D> Work { get; set; }

		[JsonIgnore]
		public Dictionary<string, string> Dictionary { get; set; }

		[JsonIgnore]
		public Dictionary<string, dynamic> ExternalVerifications { get; set; }

		public List<Workflow> Workflows { get; set; }

		[JsonIgnore]
		public List<string> Errors { get; set; }

		public List<TestCombination> MissingTests { get; set; }
	}
}

