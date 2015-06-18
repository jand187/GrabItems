using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GrabItems
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var downloader = new TabPageDownloader();
			downloader.Start(0);

			Console.ReadKey();
		}
	}
}
