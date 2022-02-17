using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary.Models.AppSpecific
{
	[Serializable]
	public class Test
	{
		public string TestID { get; set; }
		public string Server { get; set; }
		public string Path { get; set; }
		public Method Method { get; set; }
		public string Consumes { get; set; }
		public string Produces { get; set; }
		public string Body { get; set; }
		public List<string> Retain { get; set; }
		public Dictionary<string, string> Query { get; set; }
		public List<Verification> NativeVerifications { get; set; }
		public List<dynamic> ExternalVerifications { get; set; }
		public List<Result> TestResults { get; set; }

		public List<string> GetVariablePathKeys()
		{
			List<string> foundVarPath = new List<string>();
			bool found = false;
			string key = "";
			foreach (char c in Path)
			{
				if (c == '}')
				{
					found = false;
					foundVarPath.Add(key);
					key = "";
				}
				if (found) key += c;
				if (c == '{') found = true;
			}
			return foundVarPath;
		}
	}
}
