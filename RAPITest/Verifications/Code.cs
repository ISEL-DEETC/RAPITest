using Microsoft.AspNetCore.Http;
using RAPITest.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Verifications
{
	public class Code : Verification
	{
		public readonly int TargetCode;
		private const string failString = "Validation failed! Expected code: {0}, Actual code: {1}";

		public Code(int targetCode)
		{
			this.TargetCode = targetCode;
		}

		public Result Verify(HttpResponse Response)
		{
			Result res = new Result();
			res.Success = TargetCode == Response.StatusCode;

			if (!res.Success)
			{
				res.Description = String.Format(failString, TargetCode, Response.StatusCode);
			}
			return res;
		}
	}
}
