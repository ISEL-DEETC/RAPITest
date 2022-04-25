using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
	public class Test_D
	{
		public string TestID { get; set; }
		public string Server { get; set; }
		public string Path { get; set; }
		public string Method { get; set; }
		public List<string> Headers { get; set; }
		public string Body { get; set; }
		public List<string> Retain { get; set; }
		public List<string> Query { get; set; }
		public List<Verification_D> Verifications { get; set; }
	}
}
