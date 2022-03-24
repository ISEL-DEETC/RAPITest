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

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseApiSpecification
	{
		public static void Parse(CompleteTest firstTestSetup, Api api)
		{
			try
			{
				firstTestSetup.ApiSpecification = new OpenApiStreamReader().Read(new MemoryStream(api.ApiSpecification), out var diagnostic);
				if (firstTestSetup.ApiSpecification.Paths == null)
				{
					firstTestSetup.Errors.Add("Error Parsing OpenAPI Specification, please make sure it is correctly defined");
					return;
				}
				var r = new StreamReader(new MemoryStream(api.ApiSpecification));
				var deserializer = new Deserializer();
				var yamlObject = deserializer.Deserialize(r);

				Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();

				var w = new StringWriter();
				js.Serialize(w, yamlObject);
				string jsonText = w.ToString();
				JObject obj = JObject.Parse(jsonText);
				JToken tok = obj.SelectToken("$.components.schemas");

				Dictionary<string, JSchema> APISchemasAux = new Dictionary<string, JSchema>();

				if (tok.Count() > 0)
				{
					foreach (JToken schema in tok.AsEnumerable())
					{
						string originalSchema = schema.First.ToString();
						JToken tokRef = obj.SelectToken("$.components");
						string references = tokRef.Parent.ToString()+",";
						string finalSchema = originalSchema.Insert(1, references);
						APISchemasAux.Add(schema.Path.Split(".").Last(), JSchema.Parse(finalSchema));
					}
				}

				firstTestSetup.APISchemas = APISchemasAux;
			}
			catch(Exception e)
			{
				firstTestSetup.Errors.Add(e.Message);
			}
			
		}
	}
}
