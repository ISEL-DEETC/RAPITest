using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace ModelsLibrary.Models.Language
{
	public abstract class ALanguage
	{
		public static ALanguage GetLanguage(string path)
		{
			switch (path[0])
			{
				case '$':
					return new JsonLanguage();
				case '/':
					return new XMLLanguage();
				default:
					return null;
			}
		}

		public static ALanguage GetLanguage(HttpResponseMessage Response)
		{
			switch (Response.Content.Headers.ContentType.MediaType)
			{
				case "application/json":
					return new JsonLanguage();
				case "application/xml":
					return new XMLLanguage();
				default:
					return null;
			}
		}

		public static ALanguage GetLanguage(KeyValuePair<string,OpenApiMediaType> type)
		{
			switch (type.Key)
			{
				case "application/json":
					return new JsonLanguage();
				case "application/xml":
					return new XMLLanguage();
				default:
					return null;
			}
		}

		public abstract bool ValidateSchema(string schema, string obj);

		public abstract string GenerateWithSchema(string schema);

		public abstract string GetValue(string path, string obj);
	}
}
