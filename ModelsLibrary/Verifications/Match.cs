using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModelsLibrary.Models.AppSpecific;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace ModelsLibrary.Verifications
{
	public class Match : Verification
	{
		public readonly string path;
		public readonly string targetValue;
		private const string failValidationString = "Validation failed! Expected value: {0}, Actual value: {1}";
		private const string failValidationContentTypeString = "Validation failed! Expected json or xml Content Type, Actual Content Type: {0}";

		public Match(string path, string targetValue)
		{
			this.path = path;
			this.targetValue = targetValue;
		}

		public async Task<Result> Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "Match";
			res.Success = false;
			string body = await Response.Content.ReadAsStringAsync();
			string val = null;
			if (Response.Content.Headers.ContentType.MediaType == "application/json")
			{
				JObject obj = JObject.Parse(body);
				JToken value = obj.SelectToken(path);
				if(value != null)
				{
					val = value.ToString();
				}
			}
			else if(Response.Content.Headers.ContentType.MediaType == "application/xml")
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(body);
				XmlNode node = xmlDoc.SelectSingleNode(path);
				if (node != null)
				{
					val = node.InnerText;
				}
			}
			else
			{
				res.Description = String.Format(failValidationContentTypeString, Response.Content.Headers.ContentType.MediaType);
			}

			if(val == targetValue)
			{
				res.Success = true;
			}
			else
			{
				res.Description = String.Format(failValidationString, targetValue, val);
			}

			return res;
		}
	}
}
