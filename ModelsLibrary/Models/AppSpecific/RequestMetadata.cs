using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models.AppSpecific
{
	[Serializable]
	public class RequestMetadata
	{
		public string Method { get; set; }
		public string URI { get; set; }
		public IEnumerable<KeyValuePair<String, IEnumerable<String>>> Headers { get; set; }
		public string Body { get; set; }
		public int ResponseCode { get; set; }
		public string ResponseBody { get; set; }
		public IEnumerable<KeyValuePair<String, IEnumerable<String>>> ResponseHeaders { get; set; }
		public long ResponseTime { get; set; }
	}
}
