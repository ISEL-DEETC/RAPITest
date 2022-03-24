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
using System.Xml.XPath;
using ModelsLibrary.Models.Language;

namespace ModelsLibrary.Verifications
{
	public class Match : Verification
	{
		public readonly string path;
		public readonly string targetValue;

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

			ALanguage language = ALanguage.GetLanguage(Response);

			string val = language.GetValue(path, body);

			if (val != null) res.Success = true;

			return res;
		}
	}
}
