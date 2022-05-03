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
using ModelsLibrary.Models.Language;

namespace ModelsLibrary.Verifications
{
	public class Schema : Verification
	{
		public string schema;

		public Schema(string schema)
		{
			this.schema = schema;
		}

		public async Task<Result> Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "Schema";
			res.Success = false;
			try
			{
				string body = await Response.Content.ReadAsStringAsync();

				ALanguage language = ALanguage.GetLanguage(Response);

				res.Success = language.ValidateSchema(schema, body);

				if (!res.Success) res.Description = string.Join(";", language.invalidMessageErrors);
			}
			catch (Exception)
			{
				res.Success = false;
				res.Description = "Validation failed due to unkown error";
			}

			return res;
		}
		
	}

}
