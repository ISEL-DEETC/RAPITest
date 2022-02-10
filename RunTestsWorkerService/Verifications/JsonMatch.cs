using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RunTestsWorkerService.Models.AppSpecific;
using System.Net.Http;

namespace RunTestsWorkerService.Verifications
{
	public class JsonMatch : Verification
	{
		public readonly string jsonPath;
		public readonly string targetValue;
		private const string failString = "Validation failed! Expected value: {0}, Actual value: {1}";

		public JsonMatch(string jsonPath, string targetValue)
		{
			this.jsonPath = jsonPath;
			this.targetValue = targetValue;
		}

		public Result Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "JsonMatch";
			res.Success = false;

			/*if (Response.Content != "application/json")
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
				JToken value = obj.SelectToken(jsonPath);

				if(int.TryParse(targetValue, out int n))
				{
					if(n == (int)value)
					{
						res.Success = true;
					}
					else
					{
						res.Description = String.Format(failString, targetValue, n);
					}
				}
				else
				{
					if (targetValue == (string)value)
					{
						res.Success = true;
					}
					else
					{
						res.Description = String.Format(failString, targetValue, (string)value);
					}
				}
				
			}*/
			return res;
		}
	}
}
