using System;
using System.IO;
using System.Reflection;

namespace OTAPI.Tests.Common
{
	public class GameRunner
	{
		private class Config
		{
			public bool AutoStart { get; set; }
		}

		public event EventHandler PreStart;

		private Config _config;

		public void Main(string[] args)
		{
			try
			{
				_config = new Config();

				AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs sargs)
				{
					var asm = typeof(Terraria.Program).Assembly;

					var resourceName = new AssemblyName(sargs.Name).Name + ".dll";
					var text = Array.Find(asm.GetManifestResourceNames(), (string element) => element.EndsWith(resourceName));
					if (text == null)
					{
						return null;
					}

					using (Stream manifestResourceStream = asm.GetManifestResourceStream(text))
					{
						var array = new byte[manifestResourceStream.Length];
						manifestResourceStream.Read(array, 0, array.Length);
						return Assembly.Load(array);
					}
				};

				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Clear();

				Console.WriteLine("OTAPI Test Launcher.");

				var options = new NDesk.Options.OptionSet()
						.Add("as:|auto-start:", x => _config.AutoStart = true);
				options.Parse(args);

				//AttachHooks();
				PreStart?.Invoke(this, EventArgs.Empty);

				if (_config.AutoStart)
					StartGame(args);
				else Menu(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey(true);
			}
		}

		static void StartGame(string[] args)
		{
			Console.WriteLine("Starting...");
			//Terraria.Main.SkipAssemblyLoad = true;
			Terraria.WindowsLaunch.Main(args);
		}

		static void Menu(string[] args)
		{
			Console.WriteLine("Main menu:");

			var offset = 0;

			var wait = true;

			var startX = Console.CursorLeft;
			var startY = Console.CursorTop;

			var menus = new[]
			{
				new
				{
					Text = "Start game",
					Execute = new Action(() =>
					{
						StartGame(args);
						wait =false;
					})
				},
				new
				{
					Text = "Exit",
					Execute = new Action(() => { wait=false; })
				}
			};

			for (var x = 0; x < menus.Length; x++)
			{
				Console.Write(x == offset ? "\t> " : "\t  ");
				Console.WriteLine(menus[x].Text);
			}

			while (wait)
			{
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.DownArrow)
				{
					offset++;

					if (offset >= menus.Length) offset = 0;
				}
				else if (key.Key == ConsoleKey.UpArrow)
				{
					offset--;

					if (offset < 0) offset = menus.Length - 1;
				}
				else if (key.Key == ConsoleKey.Enter)
				{
					menus[offset].Execute();
				}

				Console.CursorLeft = startX;
				Console.CursorTop = startY;
				for (var x = 0; x < menus.Length; x++)
				{
					Console.Write(x == offset ? "\t> " : "\t  ");
					Console.WriteLine(menus[x].Text);
				}
			}
		}
	}
}
