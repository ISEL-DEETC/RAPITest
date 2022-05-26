using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models.AppSpecific
{
	[Serializable]
	public class APISpecificationInfo
	{
		public List<string> Servers { get; set; }
		public List<string> Paths { get; set; }
		public List<string> Schemas { get; set; }
	}
}
