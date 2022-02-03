using RAPITest.Models;
using RAPITest.Models.AppSpecific;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RAPITest.SetupTests
{
	public class SetupExternalDLLs
	{
		public static void Setup(FirstTestSetup firstTestSetup)
		{
			Dictionary<string, dynamic> externalVerirications = new Dictionary<string, dynamic>();

			string testInfoPath = Path.Combine(firstTestSetup.ApiPath, "TestInformation");
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
						firstTestSetup.Errors.Add("The supplied external validators do not comply with the expected form");
					}
				}
				if (obj != null)
				{
					dynamic v = obj as Verification ?? (dynamic)obj;
					externalVerirications.Add(file.Name, v);
				}
			}
			firstTestSetup.ExternalVerifications = externalVerirications;
		}

		private static bool CompareClasses(Type internalClass, Type externalClass)
		{
			if (internalClass.GetFields().Length != externalClass.GetFields().Length || internalClass.GetMethods().Length != externalClass.GetMethods().Length)
			{
				return false;
			}
			foreach (FieldInfo fieldInfo in internalClass.GetFields())
			{
				bool foundMatch = false;
				foreach (FieldInfo fieldInfoExternal in externalClass.GetFields())
				{
					if (fieldInfo.Name == fieldInfoExternal.Name && fieldInfo.FieldType == fieldInfoExternal.FieldType)
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
						if (parameterInfosInternal.Length == 0 && parameterInfosExternal.Length == 0)
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
								if (parameterInfoInternal.Name == parameterInfoExternal.Name && parameterInfoInternal.ParameterType == parameterInfoExternal.ParameterType)
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
	}
}
