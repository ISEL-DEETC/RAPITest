using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
using ModelsLibrary.Models.AppSpecific;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using NJsonSchema;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Text;

namespace ModelsLibrary.Verifications
{
	public class Schema : Verification
	{
		public string schema;
		private const string failStringSchemaValidation = "Validation failed! Schema does not comply with body: {0}";
		private const string failValidationContentTypeString = "Validation failed! Expected json or xml Content Type, Actual Content Type: {0}";
		private const string failStringXmlSchema = "Validation failed! Error in supplied schema";

		public Schema(string schema)
		{
			this.schema = schema;
		}

		public async Task<Result> Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "Schema";
			res.Success = false;
			string body = await Response.Content.ReadAsStringAsync();

			if (Response.Content.Headers.ContentType.MediaType == "application/json")
			{
				JObject obj = JObject.Parse(body);
				JSchema jschema = JSchema.Parse(schema);
				res.Success = obj.IsValid(jschema);
			}
			else if (Response.Content.Headers.ContentType.MediaType == "application/xml")
			{
				string xsdContent = schema;
				string xmlContent = body;
				try
				{
					XmlSchemaSet xmlschema;
					XDocument xmlDoc;
					using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xsdContent)))
					{
						var xsc = XmlSchema.Read(ms, (o, e) =>
						{
							res.Description = failStringXmlSchema;
						});
						xmlschema = new XmlSchemaSet();
						xmlschema.Add(xsc);
						xmlDoc = XDocument.Parse(xmlContent, LoadOptions.SetLineInfo);
					}

					xmlDoc.Validate(xmlschema, (o, e) =>
					{
						res.Description = String.Format(failStringSchemaValidation, body);
					});
				}
				catch (Exception) { }

				if (res.Description == null) res.Success = true;
			}
			else
			{
				res.Description = String.Format(failValidationContentTypeString, Response.Content.Headers.ContentType.MediaType);
			}

			if (!res.Success)
			{
				res.Description = String.Format(failStringSchemaValidation, body);
			}

			return res;
		}
		
	}

}
