using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using SetupTestsWorkerService.Models;
using SetupTestsWorkerService.Models.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseApiSpecification
	{
		public static void Parse(FirstTestSetup firstTestSetup, Api api)
		{

			firstTestSetup.ApiSpecification = new OpenApiStreamReader().Read(new MemoryStream(api.ApiSpecification), out var diagnostic);
			
			if (firstTestSetup.ApiSpecification.Paths == null)
			{
				firstTestSetup.Errors.Add("Error Parsing OpenAPI Specification, please make sure it is correctly defined");
			}
		}
	}
}
