using Microsoft.OpenApi.Models;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Verifications;
using SetupTestsWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SetupTestsWorkerService.SetupTests
{
	public class CheckWarnings
	{

		//step 7 - Check if any combination of server/endpoint/input/output/code isn't beeing tested
		public static void Check(CompleteTest firstTestSetup)
		{
			List<Test> testCombinations = new List<Test>();

			foreach (OpenApiServer server in firstTestSetup.ApiSpecification.Servers)
			{
				foreach (KeyValuePair<string, OpenApiPathItem> path in firstTestSetup.ApiSpecification.Paths)
				{
					foreach(KeyValuePair<OperationType, OpenApiOperation> operation in path.Value.Operations)
					{
						Enum.TryParse<Method>(operation.Key.ToString(), out Method method);
						foreach (KeyValuePair<string, OpenApiResponse> response in operation.Value.Responses)
						{
							if (response.Key == "default") continue;
							if(response.Value.Content.Count == 0)
							{
								List<Verification> nativeVerifications = new List<Verification>();
								nativeVerifications.Add(new Code(Int32.Parse(response.Key)));
								Test testCombination = new Test(server.Url, path.Key, method, null, null, null, nativeVerifications);
								testCombinations.Add(testCombination);
							}
							foreach (KeyValuePair<string, OpenApiMediaType> produces in response.Value.Content)
							{
								if (operation.Value.RequestBody != null)
								{
									foreach (KeyValuePair<string, OpenApiMediaType> consumes in operation.Value.RequestBody.Content)
									{
										List<Verification> nativeVerifications = new List<Verification>();
										nativeVerifications.Add(new Code(Int32.Parse(response.Key)));
										if (produces.Value.Schema.Reference != null)
										{
											nativeVerifications.Add(new Schema(firstTestSetup.APISchemas.GetValueOrDefault(produces.Value.Schema.Reference.ReferenceV2.Split("/").Last()).ToString()));
										}
										string body = null;
										if (consumes.Value.Schema.Reference != null)
										{
											//nao pode ser sempre json
											body = JsonSchemaSampleGenerator.Generate(firstTestSetup.APISchemas.GetValueOrDefault(consumes.Value.Schema.Reference.ReferenceV2.Split("/").Last())).ToString();
										}
										Test testCombination = new Test(server.Url, path.Key, method, produces.Key, consumes.Key, body, nativeVerifications);

										testCombinations.Add(testCombination);
									}
								}
								else
								{
									List<Verification> nativeVerifications = new List<Verification>();
									nativeVerifications.Add(new Code(Int32.Parse(response.Key)));
									if (produces.Value.Schema.Reference != null)
									{
										nativeVerifications.Add(new Schema(firstTestSetup.APISchemas.GetValueOrDefault(produces.Value.Schema.Reference.ReferenceV2.Split("/").Last()).ToString()));
									}
									Test testCombination = new Test(server.Url, path.Key, method, produces.Key, null, null, nativeVerifications);

									testCombinations.Add(testCombination);
								}
							}
						}
					}
				}
			}

			firstTestSetup.GeneratedTests = testCombinations;
			firstTestSetup.MissingTests = testCombinations;
			RemoveTests(firstTestSetup);
		}

		private static void RemoveTests(CompleteTest firstTestSetup)
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
