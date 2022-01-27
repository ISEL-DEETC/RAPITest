using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models.AppSpecific
{
	public class Test
	{
		public string TestID { get; set; }
		public string Path { get; set; }
		public Method Method { get; set; }
		public string Consumes { get; set; }
		public string Produces { get; set; }
		public string Body { get; set; }
		public List<Verification> Verifications { get; set; }
	}
}
