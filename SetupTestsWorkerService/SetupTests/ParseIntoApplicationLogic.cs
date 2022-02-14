using Microsoft.OpenApi.Models;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseIntoApplicationLogic
	{
		public static void Parse(FirstTestSetup firstTestSetup)
		{
			List<Workflow> workflows = new List<Workflow>();
			foreach (Workflow_D workflow_d in firstTestSetup.Work)
			{
				Workflow newWork = new Workflow();
				List<Test> tests = new List<Test>();

				newWork.WorkflowID = workflow_d.WorkflowID;

				foreach (Test_D test_D in workflow_d.Tests)
				{
					Test newTest = new Test();

					newTest.TestID = test_D.TestID;
					newTest.Server = test_D.Server;
					newTest.Path = test_D.Path;

					Method method;
					if (Enum.TryParse<Method>(test_D.Method, out method))
					{
						newTest.Method = method;
					}
					else
					{
						firstTestSetup.Errors.Add("Method in TSL file not supported, must be exactly Get, Post, Put or Delete. It is case sensitive!");
					}

					newTest.Consumes = test_D.Consumes;
					newTest.Produces = test_D.Produces;

					if (test_D.Body != null)
					{
						if (test_D.Body.StartsWith("$ref/dictionary/"))
						{
							string key = test_D.Body.Substring(16, test_D.Body.Length - 16);
							if (!firstTestSetup.Dictionary.TryGetValue(key, out string value))
							{
								firstTestSetup.Errors.Add("Dictionary reference in TSL file not found in dictionary file, the id must be exact, case sensitive");
							}
							newTest.Body = value;
						}
						else
						{
							newTest.Body = test_D.Body;
						}
					}

					newTest.NativeVerifications = setupNativeVerifications(test_D.Verifications, firstTestSetup);
					newTest.ExternalVerifications = setupExternalVerifications(test_D.Verifications, firstTestSetup);

					tests.Add(newTest);
				}
				newWork.Tests = tests;
				workflows.Add(newWork);
			}

			firstTestSetup.Workflows = workflows;
		}

		private static List<dynamic> setupExternalVerifications(List<Verification_D> verifications, FirstTestSetup firstTestSetup)
		{
			List<dynamic> allVerifications = new List<dynamic>();
			Verification_D verification = verifications[0];

			if (verification.Custom != null)
			{
				foreach (string key in verification.Custom)
				{
					if (!firstTestSetup.ExternalVerifications.TryGetValue(key, out dynamic value))
					{
						firstTestSetup.Errors.Add("DLL reference in TSL file not found, the file name must be exact, case sensitive");
						return null;
					}
					allVerifications.Add(value);
				}
			}

			return allVerifications;
		}

		private static List<Verification> setupNativeVerifications(List<Verification_D> verifications, FirstTestSetup firstTestSetup)
		{
			List<Verification> allVerifications = new List<Verification>();
			Verification_D verification = verifications[0];

			if (verification.Code != 0)
			{
				allVerifications.Add(new Code(verification.Code));
			}

			if (verification.JsonMatch != null)
			{
				string jsonPath = verification.JsonMatch.Split("#")[0];
				string targetValue = verification.JsonMatch.Split("#")[1];
				allVerifications.Add(new JsonMatch(jsonPath, targetValue));
			}

			if (verification.JsonSchema != null)
			{
				if (verification.JsonSchema.StartsWith("$ref/dictionary/"))
				{
					string key = verification.JsonSchema.Substring(16, verification.JsonSchema.Length-16);
					if (!firstTestSetup.Dictionary.TryGetValue(key, out string value))
					{
						firstTestSetup.Errors.Add("Dictionary reference in TSL file not found in dictionary file, the id must be exact, case sensitive");
					}
					try
					{
						allVerifications.Add(new JsonSchema(value));
					}catch(Exception e)
					{
						firstTestSetup.Errors.Add("Error in Json Schema validation:" +e.Message);
					}
				}
				//not working
				if (verification.JsonSchema.StartsWith("$ref/definitions/"))
				{
					string key = verification.JsonSchema.Substring(17, verification.JsonSchema.Length - 17);
					if (!firstTestSetup.ApiSpecification.Components.Schemas.TryGetValue(key, out OpenApiSchema value))
					{
						firstTestSetup.Errors.Add("Dictionary reference in TSL file not found in dictionary file, the id must be exact, case sensitive");
					}
					allVerifications.Add(new JsonSchema(value.ToString()));
				}
			}

			return allVerifications;
		}
	}
}
