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
			}
			return null;
		}

		public async void RunVerifications(Test test, HttpResponseMessage response)
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

		public HttpRequestMessage PrepareRequestMessage(Test test, string Path)
		{
			var query = test.Query;
			var uri = QueryHelpers.AddQueryString(test.Server + Path, query);

			var requestMessage = new HttpRequestMessage(Convert(test.Method), uri);
			
			if (test.Headers.ContainsKey("Produces") && test.Headers.GetValueOrDefault("Produces") != null) requestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(test.Headers.GetValueOrDefault("Produces")));

			if (test.Body != null) requestMessage.Content = new StringContent(test.Body, Encoding.UTF8, test.Headers.GetValueOrDefault("Consumes"));

			foreach (KeyValuePair<string, string> entry in test.Headers)
			{
				if (entry.Key.Equals("Produces") || entry.Key.Equals("Consumes")) continue;

				requestMessage.Headers.Add(entry.Key, entry.Value);
			}

			return requestMessage;
		}

		public async Task<HttpResponseMessage> Request(HttpRequestMessage request)
		{
			return await httpClient.SendAsync(request);
		}

		public async void FillRequestMetadata(Test test, HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage, long time)
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

		public void Merge(Dictionary<string, List<long>> me, Dictionary<string, List<long>> merge)
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
	}
}
