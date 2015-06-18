using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace GrabItems
{
	internal class TabPageDownloader
	{
		private Queue<DownloadSpec> _queue;

		public void Start(int index)
		{
			var numberOfTabs = GetNumberOfTabs(index);

			var downloadSpecs = Enumerable.Range(0, numberOfTabs - 1).Select(e => new DownloadSpec(e));
			_queue = new Queue<DownloadSpec>(downloadSpecs);

			while (_queue.Count > 0)
			{
				var processResult = ProcessQueue();
				if (processResult.Status == ProcessStatus.Wait)
				{
					Console.WriteLine("Error:\r\n{0}", processResult.ReturnedJSon);
					Console.WriteLine("Request limit exceeded. Readding {0} to process queue, and waiting 1 minute, before resuming.", processResult.Index);
					Thread.Sleep(60*1000);
				}

				if (processResult.Status == ProcessStatus.Ok)
				{
					SaveToDisk(processResult.ReturnedJSon, processResult.Index);
					Console.WriteLine("Process {0} sucessfully.", processResult.Index);
				}
			}
		}

		private static int GetNumberOfTabs(int index)
		{
			var message = CreateRequestMessage(index);
			var result = GetResult(message);
			var model = JsonConvert.DeserializeObject<TabModel>(result);
			var numberOfTabs = model.NumberOfTabs;
			return numberOfTabs;
		}

		private ProcessResult ProcessQueue()
		{
			var spec = _queue.Dequeue();
			var message = CreateRequestMessage(spec.Index);

			var result = GetResult(message);
			var model = JsonConvert.DeserializeObject<TabModel>(result);
			if (model.Error != null)
			{
				_queue.Enqueue(spec);
				return new ProcessResult
				{
					Status = ProcessStatus.Wait,
					Index = spec.Index,
					Model = model,
					ReturnedJSon = result,
				};
			}

			return new ProcessResult
			{
				Status = ProcessStatus.Ok,
				Index = spec.Index,
				Model = model,
				ReturnedJSon = result,
			};
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
			message.Headers.Add("Origin", "http://www.pathofexile.com");
			message.Headers.Add("Accept-Encoding", "gzip, deflate");
			message.Headers.Add("Accept-Language", "en-US,en;q=0.8,da;q=0.6");
			message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36");
			message.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
			message.Headers.Add("Referer", "http://www.pathofexile.com/");
			message.Headers.Add("X-Requested-With", "XMLHttpRequest");
			message.Headers.Add("Connection", "keep-alive");

			var content = String.Format("league=Standard&tabs=0&tabIndex={0}&accountName=PowerGNU", tabIndex);
			message.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
			return message;
		}

		private static void SaveToDisk(string result, int index)
		{
			var filename = String.Format("tab{0}.json", index);
			using (var writer = File.CreateText(Path.Combine(@"C:\Users\jda\Documents\Data", filename)))
			{
				writer.Write(result);
			}
		}
	}
}
