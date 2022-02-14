using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
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
			try
			{
				firstTestSetup.ApiSpecification = new OpenApiStreamReader().Read(new MemoryStream(api.ApiSpecification), out var diagnostic);
				if (firstTestSetup.ApiSpecification.Paths == null)
				{
					firstTestSetup.Errors.Add("Error Parsing OpenAPI Specification, please make sure it is correctly defined");
				}
			}
			catch(Exception e)
			{
				firstTestSetup.Errors.Add(e.Message);
			}
			
		}
	}
}
