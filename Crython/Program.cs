using System;
using System.Collections.Generic;
using System.Text;
using Crython.ParseTree;
using Crython.Serialization;

namespace Crython
{
	internal class Program
	{
		static void Main(string[] args)
		{
#if DEBUG
			args = new string[] { @"C:\Things\SpaceZelda\settings.crython" };
#endif

			if (args.Length != 1)
			{
				System.Console.WriteLine("Usage:");
				System.Console.WriteLine(@"  C:\MyGame> crython settings.crython");
				return;
			}

			string path = args[0];
			path = System.IO.Path.GetFullPath(path);

			if (!System.IO.File.Exists(path))
			{
				System.Console.WriteLine("Settings file not found.");
				return;
			}

#if DEBUG
			Run(path);
#else
			try
			{
				Run(path);
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
#endif
		}

		private static void Run(string settingsPath)
		{
			string workingDirectory = System.IO.Path.GetDirectoryName(settingsPath);
			Dictionary<string, string> settings = ParseSettings(settingsPath);

			string projectName = settings["PROJECT_NAME"];
			string windowTitle = settings["WINDOW_TITLE"];

			int fps;
			int gameWidth;
			int gameHeight;
			int screenWidth;
			int screenHeight;

			if (!int.TryParse(settings["FPS"], out fps)) throw new Exception("FPS was not an integer in settings.crython.");
			if (!int.TryParse(settings["GAME_WIDTH"], out gameWidth)) throw new Exception("GAME_WIDTH was not an integer in settings.crython.");
			if (!int.TryParse(settings["GAME_HEIGHT"], out gameHeight)) throw new Exception("GAME_HEIGHT was not an integer in settings.crython.");
			if (!int.TryParse(settings["SCREEN_WIDTH"], out screenWidth)) throw new Exception("SCREEN_WIDTH was not an integer in settings.crython.");
			if (!int.TryParse(settings["SCREEN_HEIGHT"], out screenHeight)) throw new Exception("SCREEN_HEIGHT was not an integer in settings.crython.");

			string gameRoot = System.IO.Path.Combine(workingDirectory, settings["GAME_ROOT_DIRECTORY"]);
			string codeFolder = System.IO.Path.Combine(gameRoot, settings["CODE_DIRECTORY"]);

			// TODO: make these plural
			string imagesFolder = System.IO.Path.Combine(gameRoot, settings["IMAGES_DIRECTORY"]);
			string audioFolder = System.IO.Path.Combine(gameRoot, settings["AUDIO_DIRECTORY"]);
			string dataFolder = System.IO.Path.Combine(gameRoot, settings["DATA_DIRECTORY"]);

			string pythonOutputDirectory = System.IO.Path.Combine(workingDirectory, settings["PYTHON_OUTPUT_DIRECTORY"]);
			string crayonOutputDirectory = System.IO.Path.Combine(workingDirectory, settings["CRAYON_OUTPUT_DIRECTORY"]);
			string jsPrefix = settings["JS_PREFIX"];

			string startSceneClassName = settings["START_SCENE"];

			string[] imageFiles = FileCrawler.Crawl(imagesFolder, ".png", ".jpg");
			string imageFilesString = string.Join("|", imageFiles).Replace('\\', '/');

			string[] audioFiles = FileCrawler.Crawl(audioFolder, ".ogg");

			string[] textFiles = FileCrawler.Crawl(dataFolder, ".txt", ".dat", ".json", ".xml");
			StringBuilder textFileStoreBuilder = new StringBuilder();
			textFileStoreBuilder.Append("{\n");
			foreach (string textFile in textFiles)
			{
				string content = Util.ReadFileFromDisk(System.IO.Path.Combine(dataFolder, textFile));
				string escapedContent = Util.InsertEscapeSequences(content);
				string key = "\"" + textFile.Replace('\\', '/') + "\"";
				textFileStoreBuilder.Append("\t");
				textFileStoreBuilder.Append(key);
				textFileStoreBuilder.Append(": \"");
				textFileStoreBuilder.Append(escapedContent);
				textFileStoreBuilder.Append("\",\n");
			}
			textFileStoreBuilder.Append("}\n");
			string textFileStore = textFileStoreBuilder.ToString();

			Dictionary<string, object> replacements = new Dictionary<string, object>()
			{
				{ "PROJECT_NAME", projectName },
				{ "FPS", fps },
				{ "GAME_WIDTH", gameWidth },
				{ "GAME_HEIGHT", gameHeight },
				{ "SCREEN_WIDTH", screenWidth },
				{ "SCREEN_HEIGHT", screenHeight },
				{ "IMAGE_FILES", imageFilesString },
				{ "IMAGES_ROOT", "'" + settings["IMAGES_DIRECTORY"] + "'"},
				{ "START_SCENE", startSceneClassName },
				{ "TEXT_FILES", textFileStore },
				{ "JS_PREFIX", jsPrefix },
				{ "WINDOW_TITLE", windowTitle },
			};

			Tokenizer tokenizer = new Tokenizer();
			ExecutableParser parser = new ExecutableParser();
			List<Executable> parseTree = new List<Executable>();
			foreach (string file in FileCrawler.Crawl(codeFolder, ".py"))
			{
				string code = Util.ReadFileFromDisk(System.IO.Path.Combine(codeFolder, file));
				code = ApplyReplacements(code, replacements);

				TokenStream tokens = tokenizer.Tokenize(file, code);
				parseTree.AddRange(parser.ParseCode(tokens));
			}

			Executable[] resolvedParseTree = Executable.ResolveBlock(parseTree);

			CrayonSerializer crayonSerializer = new CrayonSerializer();
			string crayonOutput = crayonSerializer.Serialize(resolvedParseTree);
			string crayonHeader = GetTemplateFile("header.cry", replacements);
			string crayonFooter = GetTemplateFile("footer.cry", replacements);
			crayonOutput = string.Join("\n", crayonHeader, crayonOutput, crayonFooter);

			PythonSerializer pythonSerializer = new PythonSerializer();
			string pythonOutput = pythonSerializer.Serialize(resolvedParseTree);
			string pythonHeader = GetTemplateFile("header.py", replacements);
			string pythonFooter = GetTemplateFile("footer.py", replacements);
			pythonOutput = string.Join("\n", pythonHeader, pythonOutput, pythonFooter);

			Util.WriteFile(System.IO.Path.Combine(crayonOutputDirectory, "Crayon.build"), GetTemplateFile("buildfile.xml", replacements));
			Util.WriteFile(System.IO.Path.Combine(crayonOutputDirectory, "source", "start.cry"), crayonOutput);
			Util.WriteFile(System.IO.Path.Combine(crayonOutputDirectory, "source", "gamelib.cry"), GetTemplateFile("gamelib.cry", replacements));
			Util.SyncDirectories(imagesFolder, System.IO.Path.Combine(crayonOutputDirectory, "source", settings["IMAGES_DIRECTORY"]), imageFiles);
			Util.SyncDirectories(audioFolder, System.IO.Path.Combine(crayonOutputDirectory, "source", settings["AUDIO_DIRECTORY"]), audioFiles);

			Util.WriteFile(System.IO.Path.Combine(pythonOutputDirectory, "run.py"), pythonOutput);
			Util.SyncDirectories(imagesFolder, System.IO.Path.Combine(pythonOutputDirectory, settings["IMAGES_DIRECTORY"]), imageFiles);
			Util.SyncDirectories(audioFolder, System.IO.Path.Combine(pythonOutputDirectory, settings["AUDIO_DIRECTORY"]), audioFiles);
		}

		private static Dictionary<string, string> ParseSettings(string buildFile)
		{
			string[] lines = Util.ReadFileFromDisk(buildFile).Split('\n');
			Dictionary<string, string> output = new Dictionary<string, string>();
			foreach (string line in lines)
			{
				string[] parts = line.Trim().Split(':');
				if (parts.Length > 1)
				{
					string key = parts[0].Trim();
					string value = parts[1];
					for (int i = 2; i < parts.Length; ++i)
					{
						value += ":" + parts[i];
					}
					output[key] = value.Trim();
				}
			}
			return output;
		}

		private static string GetTemplateFile(string path, Dictionary<string, object> replacements)
		{
			string output = Util.ReadFileFromAssembly("Serialization/Templates/" + path);
			return ApplyReplacements(output, replacements);
		}

		private static string ApplyReplacements(string code, Dictionary<string, object> replacements)
		{
			foreach (string key in replacements.Keys)
			{
				code = code.Replace("%%%" + key + "%%%", replacements[key].ToString());
			}
			return code;
		}
	}
}
