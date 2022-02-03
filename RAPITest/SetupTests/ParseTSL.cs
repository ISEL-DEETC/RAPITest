using RAPITest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace RAPITest.SetupTests
{
	public class ParseTSL
	{
		public static void Parse(FirstTestSetup firstTestSetup)
		{
			string testInformationPath = Path.Combine(firstTestSetup.ApiPath, "TestInformation");
			DirectoryInfo d = new DirectoryInfo(testInformationPath);

			FileInfo[] Files = d.GetFiles("*.yaml");
			string concatenatedFile = "";

			foreach (FileInfo file in Files)
			{
				concatenatedFile += File.ReadAllText(Path.Combine(testInformationPath, file.Name));
			}

			var deserializer = new DeserializerBuilder().Build();

			try
			{
				firstTestSetup.Work = deserializer.Deserialize<List<Workflow_D>>(concatenatedFile);
			}
			catch (Exception e)
			{
				firstTestSetup.Errors.Add(e.Message);
			}
		}
	}
}
