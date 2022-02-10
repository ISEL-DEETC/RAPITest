using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetupTestsWorkerService.Models
{
	public class Verification_D
	{
		public int Code { get; set; }
		public string JsonSchema { get; set; }
		public string JsonMatch { get; set; }
		public List<string> Custom { get; set; }
	}
}
