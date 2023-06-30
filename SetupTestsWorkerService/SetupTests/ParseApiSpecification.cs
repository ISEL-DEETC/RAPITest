using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Serilog;

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseApiSpecification
	{
		private static readonly ILogger _logger = Log.Logger;

		private static string CorrectBoolValues(string schemaStr)
		{
			return schemaStr.Replace("\"true\"", "true").Replace("\"false\"", "false");
        }

		public static void Parse(CompleteTest firstTestSetup, Api api)
		{
			try
			{
				firstTestSetup.ApiSpecification = new OpenApiStreamReader().Read(new MemoryStream(api.ApiSpecification), out var diagnostic);
				var errors = diagnostic.Errors;
				foreach (var error in errors)
				{
                    _logger.Information($"Diagnostic from ApiStreamReader {error.Message}");
                }
				if (firstTestSetup.ApiSpecification.Paths == null)
				{
					_logger.Warning("Error Parsing OpenAPI Specification, please make sure it is correctly defined");
					firstTestSetup.Errors.Add("Error Parsing OpenAPI Specification, please make sure it is correctly defined");
					return;
				}

				object yamlObject = null;
				try
				{
					var r = new StreamReader(new MemoryStream(api.ApiSpecification));
					var deserializer = new Deserializer();
					yamlObject = deserializer.Deserialize(r);
				}
				catch (Exception ex)
				{
					_logger.Error("Error in Deserializer from ApiSpecification...");
					throw ex;
				}

				JObject obj = null;
				JToken tok = null;
				try
				{
					Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();

					var w = new StringWriter();
					js.Serialize(w, yamlObject);
					string jsonText = w.ToString();
					obj = JObject.Parse(jsonText);
					tok = obj.SelectToken("$.components.schemas");
				}
				catch (Exception ex)
				{
					_logger.Error("Error during JsonSerializer and JsonParsing...");
					throw ex;
				}

				try
				{
					Dictionary<string, JSchema> APISchemasAux = new Dictionary<string, JSchema>();

					if (tok.Count() > 0)
					{
						foreach (JToken schema in tok.AsEnumerable())
						{
							string originalSchema = schema.First.ToString();
							JToken tokRef = obj.SelectToken("$.components");
							string references = tokRef.Parent.ToString() + ",";
							string finalSchema = originalSchema.Insert(1, references);
							finalSchema = CorrectBoolValues(finalSchema);
							APISchemasAux.Add(schema.Path.Split(".").Last(), JSchema.Parse(finalSchema));
						}
					}

					firstTestSetup.APISchemas = APISchemasAux;
				}
				catch (Exception ex)
				{
					_logger.Error("Error during JSON token parsing to construct API schemas...");
					throw ex;
				}
			}
			catch(Exception e)
			{
				_logger.Error(e.Message);
				firstTestSetup.Errors.Add(e.Message);
			}
			
		}
	}
}
