using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
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

namespace RAPITest.Utils
{
	public class GetAPISpecificationInfo
	{
		public static APISpecificationInfo GetSpecInfo(IFormFile apiSpec)
		{
			try
			{
				APISpecificationInfo ret = new APISpecificationInfo();

				OpenApiDocument specification = new OpenApiStreamReader().Read(apiSpec.OpenReadStream(), out var diagnostic);

				List<string> servers = new List<string>();
				foreach (OpenApiServer server in specification.Servers)
				{
					servers.Add(server.Url);
				}

				List<string> paths = new List<string>();
				foreach (KeyValuePair<string, OpenApiPathItem> path in specification.Paths)
				{
					paths.Add(path.Key);
				}

				var r = new StreamReader(apiSpec.OpenReadStream());
				var deserializer = new Deserializer();
				var yamlObject = deserializer.Deserialize(r);

				Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();

				var w = new StringWriter();
				js.Serialize(w, yamlObject);
				string jsonText = w.ToString();
				JObject obj = JObject.Parse(jsonText);
				JToken tok = obj.SelectToken("$.components.schemas");

				List<string> schemas = new List<string>();

				if (tok.Count() > 0)
				{
					foreach (JToken schema in tok.AsEnumerable())
					{
						schemas.Add(schema.Path.Split(".").Last());
					}
				}

				ret.Paths = paths;
				ret.Servers = servers;
				ret.Schemas = schemas;

				return ret;
			}
			catch (Exception e)
			{
				APISpecificationInfo ret = new APISpecificationInfo();
				ret.Paths = new List<string>();
				ret.Servers = new List<string>();
				ret.Schemas = new List<string>();

				return ret;
			}
		}
	}
}
