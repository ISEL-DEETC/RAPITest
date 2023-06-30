﻿using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Models.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SetupTestsWorkerService.SetupTests
{
	public class SetupExternalDLLs
	{
		private static readonly ILogger _logger = Log.Logger;

		public static void Setup(CompleteTest firstTestSetup, Api api)
		{
			Dictionary<string, dynamic> externalVerirications = new Dictionary<string, dynamic>();

			try
			{
				foreach (ExternalDll external in api.ExternalDll)
				{
					var verification = Assembly.Load(external.Dll);
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
						externalVerirications.Add(external.FileName, v);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
            }
            firstTestSetup.ExternalVerifications = externalVerirications;
		}

		private static bool CompareClasses(Type internalClass, Type externalClass)
		{
			try
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
			}
			catch (Exception ex)
			{
                _logger.Error(ex.Message);
            }
			try
			{
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
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
            }
            return true;
		}
	}
}
