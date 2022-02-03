using RAPITest.Models;
using RAPITest.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RAPITest.SetupTests
{
	public class SetupTestRun
	{
		public static void Run(string apiPath)
		{
			FirstTestSetup firstTestSetup = new FirstTestSetup();
			firstTestSetup.ApiPath = apiPath;
			firstTestSetup.Errors = new List<string>();

			//step 1 - Parse API Specification
			ParseApiSpecification.Parse(firstTestSetup);

			//step 2 - Parse TSL into Deserialize Models
			ParseTSL.Parse(firstTestSetup);

			//step 3 - Setup Dictionary
			SetupDictionary.Setup(firstTestSetup);

			//step 4 - Setup External DLL's
			SetupExternalDLLs.Setup(firstTestSetup);

			//step 5 - Parse Deserialized models into Logic Models with extra validations
			ParseIntoApplicationLogic.Parse(firstTestSetup);

			//step 6 - Check if any errors ocurred, if yes save them and return
			if(firstTestSetup.Errors.Count > 0)
			{
				WriteErrorFile(firstTestSetup);
				return;
			}

			//step 7 - Check if any combination of server/endpoint/input/output/code isn't beeing tested
			CheckWarnings.Check(firstTestSetup);

			//step 8 - Persisting model objects, so verifications and parsing isnt required for future tests
			JsonSerialization.WriteToJsonFile<FirstTestSetup>(Path.Combine(firstTestSetup.ApiPath,"FirstSetup.json"), firstTestSetup);

			//using var outputString = new StringWriter();
			//firstTestSetup.ApiSpecification.SerializeAsV3(new OpenApiJsonWriter(outputString));
			//firstTestSetup.JsonApiSpecification = outputString.ToString();
		}


		private static void WriteErrorFile(FirstTestSetup firstTestSetup)
		{
			firstTestSetup.Errors = firstTestSetup.Errors.Distinct().ToList();
			string reportsPath = Path.Combine(firstTestSetup.ApiPath, "Reports");
			using (StreamWriter sw = File.CreateText(Path.Combine(reportsPath, "error.txt")))
			{
				foreach(string error in firstTestSetup.Errors)
				{
					sw.WriteLine(error);
				}
			}
		}


		

	}
}
/*
public string TestID { get; set; }
public string Path { get; set; }
public Method Method { get; set; }
public string Consumes { get; set; }
public string Produces { get; set; }
public string Body { get; set; }
public List<Verification> Verifications { get; set; }

*/