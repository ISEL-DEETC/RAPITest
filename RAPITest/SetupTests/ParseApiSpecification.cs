using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using RAPITest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.SetupTests
{
	public class ParseApiSpecification
	{
		public static void Parse(FirstTestSetup firstTestSetup)
		{
			using (FileStream fs = File.OpenRead(Path.Combine(firstTestSetup.ApiPath, "petstorev3.json")))
			{
				firstTestSetup.ApiSpecification = new OpenApiStreamReader().Read(fs, out var diagnostic);
			}
			if (firstTestSetup.ApiSpecification.Paths == null)
			{
				firstTestSetup.Errors.Add("Error Parsing OpenAPI Specification, please make sure it is correctly defined");
			}
		}
	}
}
