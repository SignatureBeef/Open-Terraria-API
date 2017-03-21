using NDesk.Options;
using System;

namespace OTAPI.Patcher
{
	public class Program
	{
		static Engine.Patcher patcher;
		static OptionSet options;

		public static void Main(String[] args)
		{
			string sourceAsm = null;
			string modificationGlob = null;
			string outputPath = null;

			Console.WriteLine("Open Terraria API v2.0");

			if (args.Length == 0)
			{
#if SLN_CLIENT
				args = new[]
				{
					@"-in=../../../wrap/Terraria/Terraria.exe",
					@"-mod=../../../OTAPI.Modifications/OTAPI.**/bin/Debug/OTAPI.**.dll",
					@"-o=../../../OTAPI.dll"
				};
#else
				args = new[]
				{
					@"-in=../../../wrap/TerrariaServer/TerrariaServer.exe",
					@"-mod=../../../OTAPI.Modifications/OTAPI.**/bin/Debug/OTAPI.**.dll",
					@"-o=../../../OTAPI.dll"
				};
#endif
			}

			options = new OptionSet();
			options.Add("in=|source=", "specifies the source assembly to patch",
				op => sourceAsm = op);
			options.Add("mod=|modifications=", "Glob specifying the path to modification assemblies that will run against the target assembly.",
				op => modificationGlob = op);
			options.Add("o=|output=", "Specifies the output assembly that has had all modifications applied.",
				op => outputPath = op);

			options.Parse(args);

			if (string.IsNullOrEmpty(sourceAsm) == true
				|| string.IsNullOrEmpty(modificationGlob) == true)
			{
				options.WriteOptionDescriptions(Console.Out);
				return;
			}

			patcher = new Engine.Patcher(sourceAsm, new[] { modificationGlob }, outputPath);
			patcher.Run();
		}
	}
}
