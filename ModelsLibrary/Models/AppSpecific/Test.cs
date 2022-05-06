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

		public Test()
		{
			NativeVerifications = new List<Verification>();
		}

		public Test(string id, string url, string path, Method method, Dictionary<string, string> Headers, string body, List<Verification> nativeVerifications)
		{
			TestID = id;
			Body = body;
			NativeVerifications = nativeVerifications;
			Server = url;
			Path = path;
			Method = method;
			this.Headers = Headers;
			Query = new Dictionary<string, string>();
			ExternalVerifications = new List<dynamic>();
		}

		public string TestID { get; set; }
		public string Server { get; set; }
		public string Path { get; set; }
		public Method Method { get; set; }
		public Dictionary<string, string> Headers { get; set; }
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
			if (test.Headers.GetValueOrDefault("Consumes") != Headers.GetValueOrDefault("Consumes") || test.Headers.GetValueOrDefault("Produces") != Headers.GetValueOrDefault("Produces") || test.Method != Method)
			{
				return false;
			}

			if (!test.Server.Equals(Server))
			{
				return false;
			}


			char[] testPath = test.Path.ToCharArray();
			char[] myPath = Path.ToCharArray();


			while (true)
			{
				if (testPath.Length != 0 && myPath.Length == 0 || myPath.Length != 0 && testPath.Length == 0) return false;
				if (testPath.Length == 0 && myPath.Length == 0) break;

				if(testPath[0] == myPath[0])
				{
					testPath = new string(testPath).Remove(0, 1).ToCharArray();
					myPath = new string(myPath).Remove(0, 1).ToCharArray();
					continue;
				}
				
				if(testPath[0] == '{')
				{
					int end = new string(testPath).IndexOf('}');
					testPath = new string(testPath).Remove(0, end+1).ToCharArray();

					int myPathEnd = new string(myPath).IndexOf('/');
					if (myPathEnd == -1)
					{
						if (testPath.Length == 0)
						{
							break;
						}
						else
						{
							return false;
						}
					}
					myPath = new string(myPath).Remove(0, myPathEnd).ToCharArray();
					continue;
				}

				if (myPath[0] == '{')
				{
					int end = new string(myPath).IndexOf('}');
					myPath = new string(myPath).Remove(0, end+1).ToCharArray();

					int myPathEnd = new string(testPath).IndexOf('/');
					if (myPathEnd == -1)
					{
						if (myPath.Length == 0)
						{
							break;
						}
						else
						{
							return false;
						}
					}
					testPath = new string(testPath).Remove(0, myPathEnd).ToCharArray();
					continue;
				}

				return false;
			}

			Code v = (Code)test.NativeVerifications.Find((ver) => ver.GetType() == typeof(Code));

			Code n = (Code)NativeVerifications.Find((ver) => ver.GetType() == typeof(Code));

			return n.TargetCode == v.TargetCode;
		}
	}
}
