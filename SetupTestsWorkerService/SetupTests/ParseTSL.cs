using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseTSL
	{
		public static void Parse(CompleteTest firstTestSetup, Api api)
		{

			var deserializer = new DeserializerBuilder().Build();

			try
			{
				firstTestSetup.Work = deserializer.Deserialize<List<Workflow_D>>(Encoding.Default.GetString(api.Tsl));
			}
			catch (Exception e)
			{
				firstTestSetup.Errors.Add(e.Message);
			}
		}
	}
}
