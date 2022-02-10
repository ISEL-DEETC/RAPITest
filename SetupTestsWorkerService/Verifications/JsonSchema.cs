using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
using SetupTestsWorkerService.Models.AppSpecific;
using System.Net.Http;

namespace SetupTestsWorkerService.Verifications
{
	public class JsonSchema : Verification
	{
		public string schema;
		private const string failString = "Validation failed! Expected value: {0}, Actual value: {1}";

		public JsonSchema(string schema)
		{
			this.schema = schema;
		}

		public Result Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "JsonSchema";
			res.Success = false;
			/*if (Response.ContentType != "application/json")
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
				JSchema mySchema = JSchema.Parse(schema);

				if (obj.IsValid(mySchema))
				{
					res.Success = true;
				}
				else
				{
					res.Description = String.Format(failString, schema.ToString(), obj.ToString());
				}
			}*/

			return res;
		}
	}
}
