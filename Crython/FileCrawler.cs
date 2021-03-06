﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crython
{
	public static class FileCrawler
	{
		public static string[] Crawl(string rootFolder, params string[] extensions)
		{
			List<string> output = new List<string>();
			HashSet<string> extensionsLookup = new HashSet<string>();
			foreach (string extension in extensions)
			{
				string[] parts = extension.Split('.');
				string onlyExtension = parts[parts.Length - 1].ToLowerInvariant();
				extensionsLookup.Add(onlyExtension);
			}
			CrawlInternal(output, rootFolder, extensionsLookup);
			List<string> trimmedOutput = new List<string>();
			int trimLength = rootFolder.Length + 1;
			foreach (string file in output)
			{
				trimmedOutput.Add(file.Substring(trimLength));
			}
			return trimmedOutput.ToArray();
		}

		private static void CrawlInternal(List<string> output, string folder, HashSet<string> extLookup)
		{
			string[] files = System.IO.Directory.GetFiles(folder);
			foreach (string file in files)
			{
				string[] parts = file.Split('.');
				string extension = parts[parts.Length - 1].ToLowerInvariant();
				if (extLookup.Contains(extension))
				{
					output.Add(file);
				}
			}

			foreach (string directory in System.IO.Directory.GetDirectories(folder))
			{
				CrawlInternal(output, directory, extLookup);
			}
		}
	}
}
