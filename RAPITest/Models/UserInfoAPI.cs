using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models
{
	public class UserInfoAPI
	{
		public string APITitle { get; set; }
		public DateTime LatestReport { get; set; }
		public int Errors { get; set; }
		public int Warnings { get; set; }
		public DateTime NextTest { get; set; }
		public List<string> ErrorMessages { get; set; }
	}
}
