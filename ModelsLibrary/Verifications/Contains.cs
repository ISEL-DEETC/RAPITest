using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.Verifications
{
	public class Contains : Verification
	{
		public readonly string TargetString;
		private const string failString = "Validation failed! string {0} not found in response body";

		public Contains(string TargetString)
		{
			this.TargetString = TargetString;
		}
		public async Task<Result> Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "Contains";

			string body = await Response.Content.ReadAsStringAsync();

			res.Success = body.Contains(TargetString);

			if (!res.Success)
			{
				res.Description = String.Format(failString, TargetString);
			}

			return res;
		}
	}
}
