using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SetupTestsWorkerService.SetupTests
{
	public class SetupDictionary
	{
		public static void Setup(CompleteTest firstTestSetup, Api api)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			if (api.Dictionary != null)
			{
				string entireString = Encoding.Default.GetString(api.Dictionary);

				//append newline to ensure last example gets read
				entireString += Environment.NewLine;

				string id = "";
				bool foundExample = false;
				string example = "";
				var result = Regex.Split(entireString, "\r\n|\r|\n");
				foreach (var line in result)
				{
					if (line == "" && foundExample)
					{
						if (dic.ContainsKey(id))
						{
							firstTestSetup.Errors.Add("Error Parsing Dictionary, found duplicate id");
						}
						else
						{
							dic.Add(id, example);
						}
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
