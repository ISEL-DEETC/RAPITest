using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models.AppSpecific
{
	public class Result
	{
		public string TestName { get; set; }
		public bool Success { get; set; }
		public string Description { get; set; }
	}
}
