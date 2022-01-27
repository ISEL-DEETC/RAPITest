using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models.AppSpecific
{
	public abstract class Verification 
	{
		protected readonly HttpResponse Response;

		public Verification(HttpResponse response)
		{
			Response = response;
		}

		protected abstract Result Verify();
	}
}
