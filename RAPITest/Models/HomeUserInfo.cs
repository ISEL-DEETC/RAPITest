using ModelsLibrary.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models
{
	public class HomeUserInfo
	{
		public int SetupApiCount { get; set; }
		public DateTime LastLogin { get; set; }
		public List<ApiInfo> LatestActions { get; set; }
	}
}
