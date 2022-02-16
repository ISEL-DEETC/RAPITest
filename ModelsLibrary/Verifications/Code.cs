using Microsoft.AspNetCore.Http;
using ModelsLibrary.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModelsLibrary.Verifications
{
	public class Code : Verification
	{
		public readonly int TargetCode;
		private const string failString = "Validation failed! Expected code: {0}, Actual code: {1}";

		public Code(int TargetCode)
		{
			this.TargetCode = TargetCode;
		}

		public Task<Result> Verify(HttpResponseMessage Response)
		{
			Result res = new Result();
			res.TestName = "Code";
			res.Success = TargetCode == (int)Response.StatusCode;

			if (!res.Success)
			{
				res.Description = String.Format(failString, TargetCode, (int)Response.StatusCode);
			}
			return Task.FromResult(res);
		}
	}
}
