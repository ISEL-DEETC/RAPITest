using Microsoft.OpenApi.Models;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ModelsLibrary.Models.Deserialize;

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseIntoApplicationLogic
	{
		public static void Parse(CompleteTest firstTestSetup)
		{
			List<Workflow> workflows = new List<Workflow>();
			if (firstTestSetup.Work == null)
			{
				firstTestSetup.Workflows = workflows;
				return;
			}
			foreach (Workflow_D workflow_d in firstTestSetup.Work)
			{
				Workflow newWork = new Workflow();
				List<Test> tests = new List<Test>();

				newWork.WorkflowID = workflow_d.WorkflowID;
				newWork.Retain = new Dictionary<string, Retained>();

				SetupStress(workflow_d.Stress, newWork, firstTestSetup);

				foreach (Test_D test_D in workflow_d.Tests)
				{
					Test newTest = new Test();

					newTest.TestID = test_D.TestID;
					newTest.Server = test_D.Server;
					newTest.Path = test_D.Path;

					newTest.Query = SetupQuery(test_D.Query, firstTestSetup);

					newTest.Retain = test_D.Retain;
					if(newTest.Retain != null)
					{
						SetupRetain(test_D.Retain, newWork.Retain, firstTestSetup);
					}

					Method method;
					if (Enum.TryParse<Method>(test_D.Method, out method))
					{
						newTest.Method = method;
					}
					else
					{
						firstTestSetup.Errors.Add("Method in TSL file not supported, must be exactly Get, Post, Put or Delete. It is case sensitive!");
					}

					newTest.Headers = SetupHeaders(test_D.Headers, firstTestSetup);

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

					newTest.NativeVerifications = SetupNativeVerifications(test_D.Verifications, firstTestSetup);
					newTest.ExternalVerifications = SetupExternalVerifications(test_D.Verifications, firstTestSetup);

					tests.Add(newTest);
				}
				newWork.Tests = tests;
				workflows.Add(newWork);
			}

			firstTestSetup.Workflows = workflows;
			VerifyRetains(firstTestSetup);
		}

		private static Dictionary<string, string> SetupHeaders(List<string> headers, CompleteTest firstTestSetup)
		{
			Dictionary<string, string> myHeaders = new Dictionary<string, string>();
			if (headers == null) return myHeaders;

			foreach (string str in headers)
			{
				string[] parameters = str.Split(':',2);
				if (parameters.Length != 2)
				{
					firstTestSetup.Errors.Add("Header parameters not correctly supplied, must be two strings seperated by ':'");
				}
				else
				{
					myHeaders.Add(parameters[0], parameters[1]);
				}
			}
			return myHeaders;
		}

		private static void SetupStress(Stress_D stress, Workflow newWork, CompleteTest firstTestSetup)
		{
			if (stress == null) return;
			Stress newStress = new Stress();

			if(stress.Count <= 0 || stress.Delay < 0 || stress.Threads <= 0)
			{
				firstTestSetup.Errors.Add("Stress count, delay and threads must all be positive integers");
			}

			newStress.Count = stress.Count;
			newStress.Delay = stress.Delay;
			newStress.Threads = stress.Threads;

			newWork.StressTest = newStress;
		}

		private static Dictionary<string, string> SetupQuery(List<string> query, CompleteTest firstTestSetup)
		{
			Dictionary<string, string> queries = new Dictionary<string, string>();
			if (query == null) return queries;

			foreach(string str in query)
			{
				string[] parameters = str.Split('=');
				if(parameters.Length != 2)
				{
					firstTestSetup.Errors.Add("Query parameters not correctly supplied, must be two strings seperated by '='");
				}
				else
				{
					queries.Add(parameters[0], parameters[1]);
				}
			}
			return queries;
		}

		private static void VerifyRetains(CompleteTest firstTestSetup)
		{
			foreach(Workflow work in firstTestSetup.Workflows)
			{
				foreach(Test test in work.Tests)
				{
					List<string> foundVarPath = test.GetVariablePathKeys();
					foreach(string str in foundVarPath)
					{
						if (!work.Retain.TryGetValue(str, out var retained))
						{
							firstTestSetup.Errors.Add("Retain reference for path in test: "+test.TestID+ " not found");
						}
					}
				}
			}
		}

		private static void SetupRetain(List<string> retain, Dictionary<string, Retained> workflowDictionary, CompleteTest firstTestSetup)
		{
			foreach (string str in retain)
			{
				string[] keyvaluepair = str.Split('#');
				if(keyvaluepair.Length != 2)
				{
					firstTestSetup.Errors.Add("Retain field not correctly specified, need a unique '#' seperator");
					return;
				}
				workflowDictionary.Add(keyvaluepair[0], new Retained(keyvaluepair[1]));
			}
		}

		private static List<dynamic> SetupExternalVerifications(List<Verification_D> verifications, CompleteTest firstTestSetup)
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

		private static List<Verification> SetupNativeVerifications(List<Verification_D> verifications, CompleteTest firstTestSetup)
		{
			List<Verification> allVerifications = new List<Verification>();
			Verification_D verification = verifications[0];

			allVerifications.Add(new Code(verification.Code));

			if (verification.Match != null)
			{
				string[] matchStr = verification.Match.Split("#");
				if(matchStr.Length != 2)
				{
					firstTestSetup.Errors.Add("Match Verification not correctly supplied");
				}
				else
				{
					allVerifications.Add(new Match(matchStr[0], matchStr[1]));
				}
			}

			if (verification.Count != null)
			{
				string[] countStr = verification.Count.Split("#");
				if (countStr.Length != 2)
				{
					firstTestSetup.Errors.Add("Count Verification not correctly supplied");
				}
				else
				{
					allVerifications.Add(new Count(countStr[0], int.Parse(countStr[1])));
				}
			}

			if (verification.Contains != null)
			{
				allVerifications.Add(new Contains(verification.Contains));
			}

			if (verification.Schema != null)
			{
				if (verification.Schema.StartsWith("$ref/dictionary/"))
				{
					string key = verification.Schema.Substring(16, verification.Schema.Length-16);
					if (!firstTestSetup.Dictionary.TryGetValue(key, out string value))
					{
						firstTestSetup.Errors.Add("Dictionary reference in TSL file not found in dictionary file, the id must be exact, case sensitive");
					}
					else
					{
						allVerifications.Add(new Schema(value));
					}
				}
				if (verification.Schema.StartsWith("$ref/definitions/"))
				{
					string key = verification.Schema.Substring(17, verification.Schema.Length - 17);
					JSchema jSchema;
					if (!firstTestSetup.APISchemas.TryGetValue(key,out jSchema))
					{
						firstTestSetup.Errors.Add("Definitions reference in TSL file not found in specification file, the id must be exact, case sensitive");
					}
					else
					{
						allVerifications.Add(new Schema(jSchema.ToString()));
					}
				}
			}

			return allVerifications;
		}
	}
}
