using RAPITest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.SetupTests
{
	public class SetupDictionary
	{
		public static void Setup(FirstTestSetup firstTestSetup)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			string testInformationPath = Path.Combine(firstTestSetup.ApiPath, "TestInformation");
			string dictionaryPath = Path.Combine(testInformationPath, "Dictionary");
			DirectoryInfo d = new DirectoryInfo(dictionaryPath);

			FileInfo[] files = d.GetFiles("*.txt");

			if (files.Length == 1)
			{
				//append newline to ensure last example gets read
				File.AppendAllText(files[0].FullName, Environment.NewLine);

				string id = "";
				bool foundExample = false;
				string example = "";
				foreach (string line in System.IO.File.ReadLines(files[0].FullName))
				{
					if (line == "" && foundExample)
					{
						dic.Add(id, example);
						foundExample = false;
						id = "";
						example = "";
					}
					if (foundExample)
					{
						example += line;
					}
					if (line.Contains("dictionaryID:"))
					{
						id = line.Split("dictionaryID:")[1];
						foundExample = true;
					}
				}
			}

			firstTestSetup.Dictionary = dic;
		}
	}
}
