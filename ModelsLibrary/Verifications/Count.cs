using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.Verifications
{
	public class Count : Verification
	{
		public readonly int times;
		public readonly string targetString;
		private const string failString = "Validation failed! string {0} was found {1} times in body: {2}";

		public Count(string targetString, int times)
		{
			this.times = times;
			this.targetString = targetString;
		}

		public async Task<Result> Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "Count";

			string body = await Response.Content.ReadAsStringAsync();
			string replaced = body.Replace(targetString, "");

			int count = (body.Length - replaced.Length)/targetString.Length;

			if(count == times)
			{
				res.Success = true;
			}

			if (!res.Success)
			{
				res.Description = String.Format(failString, targetString, count, body);
			}

			return res;
		}
	}
}
