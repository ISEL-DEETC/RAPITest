using RAPITest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RAPITest.RunTests
{
	public class SetupTestRun
	{
		public static void Run(string apiPath)
		{
			//step 1 - Parse API Specification

			//step 2 - Parse TSL into Deserialize Models
			DirectoryInfo d = new DirectoryInfo(apiPath); 

			FileInfo[] Files = d.GetFiles("*.yaml"); 
			string concatenatedFile = "";

			foreach (FileInfo file in Files)
			{
				if (file.Name.Contains("tsl_")){
					concatenatedFile += File.ReadAllText(Path.Combine(apiPath, file.Name));
				}
			}

			var deserializer = new DeserializerBuilder().Build();

			try
			{
				List<Workflow_D> work = deserializer.Deserialize<List<Workflow_D>>(concatenatedFile);

				//step 3 - Parse Deserialized models into Logic Models with extra validations

				//step 4 - Run tests

			}
			catch (Exception e)
			{
				string reportsPath = Path.Combine(apiPath, "Reports");
				using (StreamWriter sw = File.CreateText(Path.Combine(reportsPath, "error.txt")))
				{
					sw.WriteLine(e);
				}
				return;
			}
		}
	}
}
