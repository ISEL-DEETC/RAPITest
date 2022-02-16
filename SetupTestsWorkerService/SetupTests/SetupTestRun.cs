using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SetupTestsWorkerService.SetupTests
{
	public class SetupTestRun
	{
		public static bool Run(int ApiId, RAPITestDBContext _context)
		{
			
			Api api = _context.Api.Include(api => api.ExternalDll).Where(a => a.ApiId == ApiId).FirstOrDefault();
			if (api == null) return false;

			CompleteTest firstTestSetup = new CompleteTest();
			firstTestSetup.ApiId = ApiId;
			firstTestSetup.Errors = new List<string>();

			//step 1 - Parse API Specification
			ParseApiSpecification.Parse(firstTestSetup, api);

			//step 2 - Parse TSL into Deserialize Models
			ParseTSL.Parse(firstTestSetup, api);

			//step 3 - Setup Dictionary
			SetupDictionary.Setup(firstTestSetup, api);

			//step 4 - Setup External DLL's
			SetupExternalDLLs.Setup(firstTestSetup, api);

			//step 5 - Parse Deserialized models into Logic Models with extra validations
			ParseIntoApplicationLogic.Parse(firstTestSetup);

			//step 6 - Check if any errors ocurred, if yes save them and return
			if (firstTestSetup.Errors.Count > 0)
			{
				WriteErrorFile(firstTestSetup,_context);
				_context.SaveChanges();
				return false;
			}

			//step 7 - Check if any combination of server/endpoint/input/output/code isn't beeing tested
			CheckWarnings.Check(firstTestSetup);

			//step 8 - Persisting model objects, so verifications and parsing isnt required for future tests
			api.SerializedTests = Encoding.Default.GetBytes(JsonConvert.SerializeObject(firstTestSetup, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }));
			_context.SaveChanges();
			return true;
			//using var outputString = new StringWriter();
			//firstTestSetup.ApiSpecification.SerializeAsV3(new OpenApiJsonWriter(outputString));
			//firstTestSetup.JsonApiSpecification = outputString.ToString();
		}


		private static void WriteErrorFile(CompleteTest firstTestSetup, RAPITestDBContext _context)
		{
			firstTestSetup.Errors = firstTestSetup.Errors.Distinct().ToList();
			ModelsLibrary.Models.EFModels.Report report = new ModelsLibrary.Models.EFModels.Report();
			report.ApiId = firstTestSetup.ApiId;
			report.ReportDate = DateTime.Now;
			report.ReportFile = firstTestSetup.Errors.SelectMany(s => System.Text.Encoding.Default.GetBytes(s + Environment.NewLine)).ToArray();
			_context.Report.Add(report);
		}

	}
}