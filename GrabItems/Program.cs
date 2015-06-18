using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace GrabItems
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var initialMessage = CreateRequestMessage(0);
			var initialResult = GetResult(initialMessage);
			var initianModel = JsonConvert.DeserializeObject<TabModel>(initialResult);

			var numberOfTabs = initianModel.NumberOfTabs;


			var downloader = new TabPageDownloader();
			downloader.Start(0);


			//var initialMessage = CreateRequestMessage(0);
			//var initialResult = GetResult(initialMessage);
			//SaveToDisk(initialResult, 0);

			//var initianModel = JsonConvert.DeserializeObject<TabModel>(initialResult);

			//for (var index = 1; index < initianModel.NumberOfTabs; index++)
			//{
			//	var message = CreateRequestMessage(index);
			//	var result = GetResult(message);
			//	var model = JsonConvert.DeserializeObject<TabModel>(result);

			//	if (model.Error != null || model.NumberOfTabs == 0)
			//	{
			//		Thread.Sleep(60*1000);
			//		result = GetResult(message);
			//	}

			//	SaveToDisk(result, index);

			//	Console.WriteLine("tab{0}.json, written to disk", index);
			//	//Thread.Sleep(500);
			//}

			Console.ReadKey();
		}

		private static void SaveToDisk(string result, int index)
		{
			var filename = string.Format("tab{0}.json", index);
			using (var writer = File.CreateText(Path.Combine(@"C:\Users\jda\Documents\Data", filename)))
			{
				writer.Write(result);
			}
		}

		private static string GetResult(HttpRequestMessage message)
		{
			var baseAddress = new Uri("http://www.pathofexile.com");
			using (var handler = new HttpClientHandler {UseCookies = false})
			using (var client = new HttpClient(handler) {BaseAddress = baseAddress})
			{
				return client.SendAsync(message).Result.Content.ReadAsStringAsync().Result;
			}
		}

		private static HttpRequestMessage CreateRequestMessage(int tabIndex)
		{
			var message = new HttpRequestMessage(HttpMethod.Post, "/character-window/get-stash-items");
			message.Headers.Add("Cookie", @"PHPSESSID=6d193364222926b91732859f46c1b7d9;");
			//message.Headers.Add("Cookie", @"stored_data=1; visited_overview=1; session_start=1432893567; PHPSESSID=6d193364222926b91732859f46c1b7d9; __utmt=1; __utma=183580967.1773992943.1392366154.1433232536.1433240571.143; __utmb=183580967.23.10.1433240571; __utmc=183580967; __utmz=183580967.1432723878.138.64.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not""%""20provided)");
			message.Headers.Add("Origin", "http://www.pathofexile.com");
			message.Headers.Add("Accept-Encoding", "gzip, deflate");
			message.Headers.Add("Accept-Language", "en-US,en;q=0.8,da;q=0.6");
			message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36");
			message.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
			message.Headers.Add("Referer", "http://www.pathofexile.com/");
			message.Headers.Add("X-Requested-With", "XMLHttpRequest");
			message.Headers.Add("Connection", "keep-alive");

			var content = string.Format("league=Standard&tabs=0&tabIndex={0}&accountName=PowerGNU", tabIndex);
			message.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
			return message;
		}
	}

	internal class TabModel
	{
		[JsonProperty("numTabs", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public int NumberOfTabs
		{
			get;
			set;
		}

		[JsonProperty("error")]
		public Error Error
		{
			get;
			set;
		}
	}

	internal class Error
	{
		[JsonProperty("message")]
		public string Message
		{
			get;
			set;
		}
	}
}
