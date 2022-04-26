using Newtonsoft.Json;
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
			try
			{
				//Test if its a Single Object
				JObject jobj = JObject.Parse(obj);
				JToken value = jobj.SelectToken(path);
				if (value != null)
				{
					return value.ToString();
				}
			}
			catch(JsonReaderException)
			{
				//Test if its a Json Array
				JArray jArray = JArray.Parse(obj);
				JToken v = jArray.SelectToken(path);
				if (v != null)
				{
					return v.ToString();
				}
			}
			return null;
		}

		public override bool ValidateSchema(string schema, string obj)
		{
			JSchema jschema = JSchema.Parse(schema);
			try
			{
				//Test if its a Single Object
				JObject jobj = JObject.Parse(obj);
				return jobj.IsValid(jschema, out invalidMessageErrors);
			}
			catch (JsonReaderException)
			{
				//Test if its a Json Array
				JArray jArray = JArray.Parse(obj);
				return jArray.IsValid(jschema, out invalidMessageErrors);
			}
		}
	}
}
