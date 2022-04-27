using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models.AppSpecific
{
	public class VisualizeReportModel
	{
		public string ApiName { get; set; }
		public string Report { get; set; }
		public List<DateTime> AllReportDates { get; set; }
	}
}
