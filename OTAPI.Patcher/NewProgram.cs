using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTAPI.Patcher.Engine;

namespace OTAPI.Patcher
{
	public class NewProgram
	{
		static Engine.Patcher patcher;
		static OptionSet options;

		public static void Main(String[] args)
		{
			string sourceAsm = null;
			string modificationGlob = null;

			Console.WriteLine("Open Terraria API v2.0");

			options = new OptionSet();
			options.Add("in=|source=", "specifies the source assembly to patch", 
				op => sourceAsm = op);
			options.Add("mod=|modifications=", "Glob specifiying the path to modification assemblies that will run against the target assembly.",
				op => modificationGlob = op);

			options.Parse(args);

			if (string.IsNullOrEmpty(sourceAsm) == true
				|| string.IsNullOrEmpty(modificationGlob) == true)
			{
				options.WriteOptionDescriptions(Console.Out);
				return;
			}

			patcher = new Engine.Patcher(sourceAsm, modificationGlob);
			patcher.Run();
		}
	}
}
