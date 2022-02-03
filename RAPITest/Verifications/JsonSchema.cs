using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
using RAPITest.Models.AppSpecific;

namespace RAPITest.Verifications
{
	public class JsonSchema : Verification
	{
		private JSchema schema;
		private const string failString = "Validation failed! Expected value: {0}, Actual value: {1}";

		public JsonSchema(string schema)
		{
			this.schema = JSchema.Parse(schema);
		}

		public Result Verify(HttpResponse Response)
		{
			Result res = new Result();
			res.Success = false;

			if (Response.ContentType != "application/json")
			{
				res.Description = "Content type wasn't in json, actual content type: " + Response.ContentType;
			}
			else
			{
				string body = string.Empty;
				using (var reader = new StreamReader(Response.Body))
				{
					Response.Body.Seek(0, SeekOrigin.Begin);
					body = reader.ReadToEnd();
				}
				JObject obj = JObject.Parse(body);

				if (obj.IsValid(schema))
				{
					res.Success = true;
				}
				else
				{
					res.Description = String.Format(failString, schema.ToString(), obj.ToString());
				}
			}

			return res;
		}
	}
}
