using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using Newtonsoft.Json;
using RunTestsWorkerService.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RunTestsWorkerService.RunTests
{
	public class RunApiTests
	{
		public static async Task RunAsync(int ApiId, string connectionString)
		{
			RAPITestDBContext _context;
			var optionsBuilder = new DbContextOptionsBuilder<RAPITestDBContext>();
			optionsBuilder.UseSqlServer(connectionString);

			using (_context = new RAPITestDBContext(optionsBuilder.Options))
			{
				Api api = _context.Api.Include(api => api.ExternalDll).Include(api => api.Report).Where(a => a.ApiId == ApiId).FirstOrDefault();
				if (api == null) return;

				//step 1 - Reload Assemblies
				foreach (ExternalDll external in api.ExternalDll)
				{
					Assembly.Load(external.Dll);
				}
				string serializedTests = Encoding.Default.GetString(api.SerializedTests);
				serializedTests = serializedTests.Replace("SetupTestsWorkerService", "RunTestsWorkerService");

				//step 2 - Parse AplicationModel serialized objects
				CompleteTest firstTestSetup = JsonConvert.DeserializeObject<CompleteTest>(serializedTests, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

				//step 3 - Run
				await MakeRequests.Make(firstTestSetup, api);

				_context.SaveChanges();
			}
		}
	}
}
