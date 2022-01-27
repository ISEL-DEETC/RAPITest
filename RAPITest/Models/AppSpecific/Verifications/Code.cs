using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models.AppSpecific.Verifications
{
	public class Code : Verification
	{
		private readonly int TargetCode;
		private const string failString = "Validation failed! Expected code: {0}, Actual code: {1}";

		public Code(HttpResponse response, int targetCode) : base(response)
		{
			this.TargetCode = targetCode;
		}

		protected override Result Verify()
		{
			Result res = new Result();
			res.Success = TargetCode == base.Response.StatusCode;

			if (!res.Success)
			{
				res.Description = String.Format(failString, TargetCode, base.Response.StatusCode);
			}
			return res;
		}
	}
}
