using RAPITest.Models;
using RAPITest.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.RunTests
{
	public class RunApiTests
	{
		public static void Run(string apiPath)
		{
			//step 1 - Parse AplicationModel serialized objects
			FirstTestSetup firstTestSetup = JsonSerialization.ReadFromJsonFile<FirstTestSetup>(Path.Combine(apiPath, "FirstSetup.json"));

			//step 2 - Run
			MakeRequests.Make(firstTestSetup);
		}
	}
}
