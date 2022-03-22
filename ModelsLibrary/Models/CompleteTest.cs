using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace ModelsLibrary.Models
{
	public class CompleteTest
	{
		public int ApiId { get; set; }

		[JsonIgnore]
		public OpenApiDocument ApiSpecification { get; set; }

		[JsonIgnore]
		public Dictionary<string, JSchema> APISchemas { get; set; }

		[JsonIgnore]
		public List<Workflow_D> Work { get; set; }

		[JsonIgnore]
		public Dictionary<string, string> Dictionary { get; set; }

		[JsonIgnore]
		public Dictionary<string, dynamic> ExternalVerifications { get; set; }

		public List<Workflow> Workflows { get; set; }

		[JsonIgnore]
		public List<string> Errors { get; set; }

		public List<Test> MissingTests { get; set; }

		public List<Test> GeneratedTests { get; set; }
	}
}

