using ModelsLibrary.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary.Models.AppSpecific
{
	[Serializable]
	public class Test
	{

		public Test(string server, string path, Method method, string consumes, string produces, List<Verification> verifications)
		{
			Server = server;
			Path = path;
			Method = method;
			Consumes = consumes;
			Produces = produces;
			NativeVerifications = verifications;
		}

		public Test()
		{
			NativeVerifications = new List<Verification>();
		}

		public Test(string id, string url, string path, Method method, string produces, string consumes, string body, List<Verification> nativeVerifications)
		{
			TestID = id;
			Body = body;
			NativeVerifications = nativeVerifications;
			Server = url;
			Path = path;
			Method = method;
			Produces = produces;
			Consumes = consumes;
			Query = new Dictionary<string, string>();
			ExternalVerifications = new List<dynamic>();
		}

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
		public RequestMetadata RequestMetadata { get; set; }
		public List<long> StressTimes { get; set; }

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


		public bool CompareTests(Test test)
		{
			if (test.Consumes != Consumes || test.Produces != Produces || test.Method != Method)
			{
				return false;
			}

			string fullPathComb = Server + Path;
			string fullPathTest = test.Server + test.Path;

			if (fullPathComb != fullPathTest)
			{
				return false;
			}

			Code v = (Code)test.NativeVerifications.Find((ver) => ver.GetType() == typeof(Code));

			Code n = (Code)NativeVerifications.Find((ver) => ver.GetType() == typeof(Code));

			return n.TargetCode == v.TargetCode;
		}
	}
}
