using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models.Language
{
	class JsonLanguage : ALanguage
	{
		public override string GenerateWithSchema(string schema)
		{
			JSchema jschema = JSchema.Parse(schema);
			return JsonSchemaSampleGenerator.Generate(jschema).ToString();
		}

		public override string GetValue(string path, string obj)
		{
			JObject jobj = JObject.Parse(obj);
			JToken value = jobj.SelectToken(path);
			if (value != null)
			{
				return value.ToString();
			}
			return null;
		}

		public override bool ValidateSchema(string schema, string obj)
		{
			JObject jobj = JObject.Parse(obj);
			JSchema jschema = JSchema.Parse(schema);
			return jobj.IsValid(jschema);
		}
	}
}
