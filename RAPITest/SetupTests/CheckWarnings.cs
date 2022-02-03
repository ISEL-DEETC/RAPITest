using Microsoft.OpenApi.Models;
using RAPITest.Models;
using RAPITest.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.SetupTests
{
	public class CheckWarnings
	{

		//step 7 - Check if any combination of server/endpoint/input/output/code isn't beeing tested
		public static void Check(FirstTestSetup firstTestSetup)
		{
			List<TestCombination> testCombinations = new List<TestCombination>();

			foreach (OpenApiServer server in firstTestSetup.ApiSpecification.Servers)
			{

				TestCombination testCombination = new TestCombination();
				testCombination.Server = server.Url;

				foreach (KeyValuePair<string, OpenApiPathItem> path in firstTestSetup.ApiSpecification.Paths)
				{
					testCombination.Endpoint = path.Key;
					

					foreach(KeyValuePair<OperationType, OpenApiOperation> operation in path.Value.Operations)
					{
						Method method;
						Enum.TryParse<Method>(operation.Key.ToString(), out method);
						testCombination.Method = method;

						foreach (KeyValuePair<string, OpenApiResponse> response in operation.Value.Responses)
						{
							if (response.Key == "default") continue;
							testCombination.ResponseCode = Int32.Parse(response.Key);
							
							foreach (KeyValuePair<string, OpenApiMediaType> produces in response.Value.Content)
							{
								testCombination.Produces = produces.Key;

								if (operation.Value.RequestBody != null)
								{
									foreach (KeyValuePair<string, OpenApiMediaType> consumes in operation.Value.RequestBody.Content)
									{
										testCombination = testCombination.Clone(testCombination);
										testCombination.Consumes = consumes.Key;
										testCombinations.Add(testCombination);
									}
								}
								else
								{
									testCombination = testCombination.Clone(testCombination);
									testCombination.Consumes = null;
									testCombinations.Add(testCombination);
								}
							}
						}
					}
				}
			}

			firstTestSetup.MissingTests = testCombinations;
			RemoveTests(firstTestSetup);
		}

		private static void RemoveTests(FirstTestSetup firstTestSetup)
		{
			foreach(Workflow worflow in firstTestSetup.Workflows)
			{
				foreach(Test test in worflow.Tests)
				{
					firstTestSetup.MissingTests.RemoveAll(item => item.CompareTests(test));
				}
			}
		}
	}
}
