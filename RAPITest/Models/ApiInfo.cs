using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models
{
	public class ApiInfo
	{
		public int ApiId { get; set; }
		public string Title { get; set; }
		public DateTime ReportDate { get; set; }
		public DateTime NextTest { get; set; }
	}
}
