using Microsoft.OpenApi.Models;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Models.Language;
using ModelsLibrary.Verifications;
using System;
using System.Collections;
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

			List<string> servers = new List<string>();
			foreach (OpenApiServer server in firstTestSetup.ApiSpecification.Servers)
			{
				servers.Add(server.Url);
			}

			foreach (KeyValuePair<string, OpenApiPathItem> path in firstTestSetup.ApiSpecification.Paths)
			{
				foreach (KeyValuePair<OperationType, OpenApiOperation> operation in path.Value.Operations)
				{
					IDictionary<string, OpenApiMediaType> consumes = new Dictionary<string, OpenApiMediaType>();
					if(operation.Value.RequestBody != null)
						consumes= operation.Value.RequestBody.Content;
					foreach (KeyValuePair<string, OpenApiResponse> response in operation.Value.Responses)
					{
						if (response.Key == "default") continue;
						List<int> responseCodes = new List<int>();
						responseCodes.Add(Int32.Parse(response.Key));
						IDictionary<string, OpenApiMediaType> responseBodyTypes = response.Value.Content;

						IEnumerable<AuxTest> cartesianProduct = from server in servers
										       from consume in consumes.DefaultIfEmpty()
										       from code in responseCodes
										       from responseType in responseBodyTypes.DefaultIfEmpty()
											   select new AuxTest(server,consume,code,responseType);

						Enum.TryParse<Method>(operation.Key.ToString(), out Method method);
						testCombinations.AddRange(GenerateTests(cartesianProduct, path.Key, method, firstTestSetup));

						Console.WriteLine("boas");
					}
				}
			}

			firstTestSetup.GeneratedTests = testCombinations;
			firstTestSetup.MissingTests = testCombinations;
			RemoveTests(firstTestSetup);
		}

		private static IEnumerable<Test> GenerateTests(IEnumerable<AuxTest> cartesianProduct, string path, Method method, CompleteTest firstTestSetup)
		{
			List<Test> newTests = new List<Test>();
			foreach(AuxTest test in cartesianProduct)
			{
				List<Verification> nativeVerifications = new List<Verification>();
				nativeVerifications.Add(new Code(test.Code));

				string body = null;
				if (test.Consumes.Key != null)
				{
					ALanguage language = ALanguage.GetLanguage(test.Consumes);
					//language not yet supported
					if (language != null)
					{
						if (test.Consumes.Value.Schema.Reference != null)
						{
							body = language.GenerateWithSchema(firstTestSetup.APISchemas.GetValueOrDefault(test.Consumes.Value.Schema.Reference.ReferenceV2.Split("/").Last()).ToString());
						}
					}
				}

				if (test.ResponseTypes.Key != null)
				{
					string schema;

					if (test.ResponseTypes.Value.Schema.Reference != null)
					{
						schema = test.ResponseTypes.Value.Schema.Reference.ReferenceV2.Split("/").Last();
						nativeVerifications.Add(new Schema(firstTestSetup.APISchemas.GetValueOrDefault(schema).ToString()));
					}
				}

				string testId = test.Server + path + method + test.ResponseTypes.Key + test.Consumes.Key;
				newTests.Add(new Test(testId, test.Server, path, method, test.ResponseTypes.Key, test.Consumes.Key, body, nativeVerifications));
			}
			return newTests;
		}

		private static void RemoveTests(CompleteTest firstTestSetup)
		{
			foreach (Workflow worflow in firstTestSetup.Workflows)
			{
				foreach (Test test in worflow.Tests)
				{
					firstTestSetup.MissingTests.RemoveAll(item => item.CompareTests(test));
				}
			}
		}

		private class AuxTest
		{
			public AuxTest(string server, KeyValuePair<string, OpenApiMediaType> consume, int code, KeyValuePair<string, OpenApiMediaType> responseType)
			{
				Server = server;
				Code = code;
				Consumes = consume;
				ResponseTypes = responseType;
			}

			public string Server { get; set; }
			public KeyValuePair<string, OpenApiMediaType> Consumes { get; set; }
			public int Code { get; set; }
			public KeyValuePair<string, OpenApiMediaType> ResponseTypes { get; set; }
		}
	}
}
