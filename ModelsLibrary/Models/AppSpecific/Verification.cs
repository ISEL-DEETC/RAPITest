using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModelsLibrary.Models.AppSpecific
{
	public interface Verification 
	{
		Result Verify(HttpResponseMessage Response);
	}
}
