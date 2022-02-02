using Microsoft.OpenApi.Readers;
using RAPITest.Models;
using RAPITest.Models.AppSpecific;
using RAPITest.Models.AppSpecific.Verifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace RAPITest.RunTests
{
	public class SetupTestRun
	{
		public static void Run(string apiPath)
		{
			//step 1 - Parse API Specification

			OpenApiDocument openApiDocument = parseApiSpecification(apiPath);
			if(openApiDocument.Paths == null)
			{
				WriteErrorFile("Error Parsing OpenAPI Specification, please make sure it is correctly defined", apiPath);
				return;
			}

			//step 2 - Parse TSL into Deserialize Models

			List<Workflow_D> work = TSLtoDeserializeModels(apiPath);
			if (work == null) return;

			//step 3 - Setup Dictionary
			Dictionary<string, string> dictionary = SetupDictionary(apiPath);

			//step 4 - Setup External DLL's
			Dictionary<string, dynamic> externalVerifications = SetupExternalVerifications(apiPath);
			if (work == null) return;

			//step 5 - Parse Deserialized models into Logic Models with extra validations
			List<Workflow> workflows = DeserializedModelsToAppSpecific(work,apiPath);

			//step 6 - Persisting model objects, so verifications and parsing isnt required for future tests

			//step 7 - Run tests

		}

		private static Dictionary<string, dynamic> SetupExternalVerifications(string apiPath)
		{
			Dictionary<string, dynamic> externalVerirications = new Dictionary<string, dynamic>();

			string testInfoPath = Path.Combine(apiPath, "TestInformation");
			string dllPath = Path.Combine(testInfoPath, "DLLs");

			DirectoryInfo d = new DirectoryInfo(dllPath);

			FileInfo[] Files = d.GetFiles("*.dll");

			foreach (FileInfo file in Files)
			{
				var verification = Assembly.LoadFile(file.FullName);
				object obj = null;
				foreach (Type type in verification.GetExportedTypes())
				{
					bool validClass = false;
					switch (type.Name)
					{
						case "Result":
							validClass = CompareClasses(typeof(Result), type);
							break;
						case "Verification":
							validClass = CompareClasses(typeof(Verification), type);
							break;
						default:
							validClass = type.GetInterface("Verification") != null;
							if (validClass)
							{
								obj = Activator.CreateInstance(type);
							}
							break;
					}
					if (!validClass)
					{
						obj = null;
						WriteErrorFile("The supplied external validators do not comply with the expected form", apiPath);
						return null;
					}
				}
				if(obj != null)
				{
					dynamic v = obj as Verification ?? (dynamic)obj;
					externalVerirications.Add(file.Name, v);
				}
			}
			return externalVerirications;
		}

		private static bool CompareClasses(Type internalClass, Type externalClass)
		{
			if(internalClass.GetFields().Length != externalClass.GetFields().Length || internalClass.GetMethods().Length != externalClass.GetMethods().Length)
			{
				return false;
			}
			foreach(FieldInfo fieldInfo in internalClass.GetFields())
			{
				bool foundMatch = false;
				foreach (FieldInfo fieldInfoExternal in externalClass.GetFields())
				{
					if(fieldInfo.Name == fieldInfoExternal.Name && fieldInfo.FieldType == fieldInfoExternal.FieldType)
					{
						foundMatch = true;
						break;
					}
				}
				if (!foundMatch) return false;
			}
			foreach (MethodInfo methodInfo in internalClass.GetMethods())
			{
				bool foundMatch = false;
				foreach (MethodInfo methodInfoExternal in externalClass.GetMethods())
				{
					if (methodInfo.Name == methodInfoExternal.Name)
					{
						ParameterInfo[] parameterInfosInternal = methodInfo.GetParameters();
						ParameterInfo[] parameterInfosExternal = methodInfoExternal.GetParameters();
						if(parameterInfosInternal.Length == 0 && parameterInfosExternal.Length == 0)
						{
							foundMatch = true;
							break;
						}
						if (parameterInfosInternal.Length != parameterInfosExternal.Length)
						{
							continue;
						}
						bool foundMatchParameter = false;
						foreach (ParameterInfo parameterInfoInternal in parameterInfosInternal)
						{
							foreach (ParameterInfo parameterInfoExternal in parameterInfosExternal)
							{
								if(parameterInfoInternal.Name == parameterInfoExternal.Name && parameterInfoInternal.ParameterType == parameterInfoExternal.ParameterType)
								{
									foundMatchParameter = true;
									break;
								}
							}
							if (!foundMatchParameter) return false;
						}
						if (foundMatchParameter)
						{
							foundMatch = true;
							break;
						}
					}
				}
				if (!foundMatch) return false;
			}
			return true;
		}

		private static OpenApiDocument parseApiSpecification(string apiPath)
		{
			using (FileStream fs = File.OpenRead(Path.Combine(apiPath,"apiSpecification.yaml")))
			{
				return new OpenApiStreamReader().Read(fs, out var diagnostic);
			}
		}

		private static List<Workflow_D> TSLtoDeserializeModels(string apiPath)
		{
			string testInformationPath = Path.Combine(apiPath, "TestInformation");
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
				List<Workflow_D> work = deserializer.Deserialize<List<Workflow_D>>(concatenatedFile);
				return work;
			}
			catch (Exception e)
			{
				WriteErrorFile(e.Message, apiPath);
				return null;
			}
		}


		private static List<Workflow> DeserializedModelsToAppSpecific(List<Workflow_D> work, string apiPath)
		{
			List<Workflow> workflows = new List<Workflow>();
			foreach (Workflow_D workflow_d in work)
			{
				Workflow newWork = new Workflow();

				newWork.WorkflowID = workflow_d.WorkflowID;
				
				foreach(Test_D test_D in workflow_d.Tests)
				{
					Test newTest = new Test();

					newTest.TestID = test_D.TestID;
					newTest.Path = test_D.Path;

					Method method;
					if(Enum.TryParse<Method>(test_D.Method,out method))
					{
						newTest.Method = method;
					}
					else
					{
						return null;
					}

					newTest.Consumes = test_D.Consumes;
					newTest.Produces = test_D.Produces;

					if(test_D.Body != null)
					{
						if (test_D.Body.StartsWith("$ref/dictionary/"))
						{

						}
					}

					newTest.Verifications = setupVerifications(test_D.Verifications);
				}
			}

			return workflows;
		}

		private static List<Verification> setupVerifications(List<Verification_D> verifications)
		{
			List<Verification> allVerifications = new List<Verification>();
			Verification_D verification = verifications[0];

			if(verification.Code != 0)
			{
				allVerifications.Add(new Code(verification.Code));
			}

			if (verification.JsonMatch != null)
			{

			}

			if (verification.JsonSchema != null)
			{

			}

			if (verification.Custom != null)
			{

			}

			return allVerifications;
		}

		private static void WriteErrorFile(string error, string apiPath)
		{
			string reportsPath = Path.Combine(apiPath, "Reports");
			using (StreamWriter sw = File.CreateText(Path.Combine(reportsPath, "error.txt")))
			{
				sw.WriteLine(error);
			}
		}


		private static Dictionary<string, string> SetupDictionary(string apiPath)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			string testInformationPath = Path.Combine(apiPath, "TestInformation");
			string dictionaryPath = Path.Combine(testInformationPath, "Dictionary");
			DirectoryInfo d = new DirectoryInfo(dictionaryPath);

			FileInfo[] files = d.GetFiles("*.txt");

			if(files.Length == 1)
			{
				//append newline to ensure last example gets read
				File.AppendAllText(files[0].FullName, Environment.NewLine);

				string id = "";
				bool foundExample = false;
				string example = "";
				foreach (string line in System.IO.File.ReadLines(files[0].FullName))
				{
					if(line == "" && foundExample)
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

			return dic;
		}

	}
}
/*
public string TestID { get; set; }
public string Path { get; set; }
public Method Method { get; set; }
public string Consumes { get; set; }
public string Produces { get; set; }
public string Body { get; set; }
public List<Verification> Verifications { get; set; }

*/