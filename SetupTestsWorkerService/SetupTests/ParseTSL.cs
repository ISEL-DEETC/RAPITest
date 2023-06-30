using ModelsLibrary.Models;
using ModelsLibrary.Models.EFModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Serilog;

namespace SetupTestsWorkerService.SetupTests
{
	public class ParseTSL
	{
		private static readonly ILogger _logger = Log.Logger;

		public static void Parse(CompleteTest firstTestSetup, Api api)
		{
			IDeserializer deserializer = null;
			try
			{
				deserializer = new DeserializerBuilder().Build();
			}
			catch (Exception ex)
			{
				_logger.Error("Error building Deserializer");
				_logger.Error(ex.Message);
			}

			try
			{
				firstTestSetup.Work = deserializer.Deserialize<List<Workflow_D>>(Encoding.Default.GetString(api.Tsl));
			}
			catch (Exception e)
			{
				_logger.Error("Error deserializing TSL");
				_logger.Error(e.Message);
				firstTestSetup.Errors.Add(e.Message);
			}
		}
	}
}
