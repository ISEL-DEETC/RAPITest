using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using ModelsLibrary.Models.AppSpecific;
using ModelsLibrary.Models.EFModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RunTestsWorkerService.Utils;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using ModelsLibrary.Models.Language;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace RunTestsWorkerService.Utils
{
	sealed class HttpUtils
	{
		private static readonly HttpClient httpClient = new HttpClient();

		private HttpUtils() { }

		private static HttpUtils _instance;

		public static HttpUtils GetInstance()
		{
			if (_instance == null)
			{
				_instance = new HttpUtils();
			}
			return _instance;
		}

		public HttpMethod Convert(Method method)
		{
			switch (method)
			{
				case (Method.Get):
					return HttpMethod.Get;
				case (Method.Post):
					return HttpMethod.Post;
				case (Method.Put):
					return HttpMethod.Put;
				case (Method.Delete):
					return HttpMethod.Delete;
				default:
					Log.Logger.Warning($"Should never go here {method}");
					break;

			}
			return null;
		}

		public async void RunVerifications(Test test, HttpResponseMessage response)
		{
			try
			{

				if (test.TestResults == null) test.TestResults = new List<Result>();
				foreach (Verification verification in test.NativeVerifications)
				{
					Result r = await verification.Verify(response);
					test.TestResults.Add(r);
				}
				foreach (dynamic verification in test.ExternalVerifications)
				{
					Result r = new Result();
					dynamic result = await verification.Verify(response);
					r.Success = result.Success;
					r.Description = result.Description;
					r.TestName = result.TestName;
					test.TestResults.Add(r);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex.Message);
			}
		}

		public HttpRequestMessage PrepareRequestMessage(Test test, string Path)
		{
			try
			{
				var query = test.Query;
				var uri = QueryHelpers.AddQueryString(test.Server + Path, query);

				var requestMessage = new HttpRequestMessage(Convert(test.Method), uri);

				if (test.Headers.ContainsKey(HeaderNames.Accept) && test.Headers.GetValueOrDefault(HeaderNames.Accept) != null) requestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(test.Headers.GetValueOrDefault(HeaderNames.Accept)));

				if (test.Body != null) requestMessage.Content = new StringContent(test.Body, Encoding.UTF8, test.Headers.GetValueOrDefault(HeaderNames.ContentType));

				foreach (KeyValuePair<string, string> entry in test.Headers)
				{
					if (entry.Key.Equals(HeaderNames.Accept) || entry.Key.Equals(HeaderNames.ContentType)) continue;

					requestMessage.Headers.Add(entry.Key, entry.Value);
				}

				return requestMessage;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex.Message);
				return null;
			}
		}

		public async Task<HttpResponseMessage> Request(HttpRequestMessage request)
		{
			try
			{
				return await httpClient.SendAsync(request);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"[HttpUtils].[Request] {ex.Message}");
				throw ex;
			}
		}

		public async void FillRequestMetadata(Test test, HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage, long time)
		{
			try
			{
				RequestMetadata requestMetadata = new RequestMetadata();
				requestMetadata.Method = httpRequestMessage.Method.Method;
				requestMetadata.URI = httpRequestMessage.RequestUri.AbsoluteUri;
				if (httpRequestMessage.Content != null)
				{
					requestMetadata.Headers = httpRequestMessage.Headers.Concat(httpRequestMessage.Content.Headers);
					requestMetadata.Body = await httpRequestMessage.Content.ReadAsStringAsync();
				}
				requestMetadata.ResponseCode = (int)httpResponseMessage.StatusCode;
				requestMetadata.ResponseHeaders = httpResponseMessage.Headers.Concat(httpResponseMessage.Content.Headers);
				requestMetadata.ResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
				requestMetadata.ResponseTime = time;

				test.RequestMetadata = requestMetadata;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"[HttpUtils].[FillRequestMetadata] {ex.Message}");
			}
		}

		public void Merge(Dictionary<string, List<long>> me, Dictionary<string, List<long>> merge)
		{
			try
			{
				foreach (var item in merge)
				{
					if (!me.ContainsKey(item.Key))
					{
						me[item.Key] = item.Value;
					}
					else
					{
						me[item.Key].AddRange(item.Value);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"[HttpUtils].[Merge] {ex.Message}");
				throw ex;
			}
		}
	}
}
