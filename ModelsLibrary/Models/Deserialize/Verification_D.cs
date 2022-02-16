using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
	public class Verification_D
	{
		public int Code { get; set; }
		public string Schema { get; set; }
		public string Match { get; set; }
		public string Contains { get; set; }
		public string Count { get; set; }
		public List<string> Custom { get; set; }
	}
}
