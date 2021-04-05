using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OTAPI.Patcher
{
	public class Program
	{
		static Engine.Patcher patcher;
		static OptionSet options;

		public static void Main(String[] args)
		{
			string sourceAsm = null;
			string outputPath = null;
			List<string> mergeInputs = new List<string>();
			List<string> modificationGlobs = new List<string>();
			string mergeOutput = null;

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
					@"-pre-merge-in=../../../wrap/TerrariaServer/TerrariaServer.exe",
#if DEBUG
					@"-pre-merge-in=../../../OTAPI.Modifications/SteelSeriesEngineWrapper/bin/Debug/SteelSeriesEngineWrapper.dll",
#else
					@"-pre-merge-in=../../../OTAPI.Modifications/SteelSeriesEngineWrapper/bin/Release/SteelSeriesEngineWrapper.dll",
#endif
					@"-pre-merge-in=../../../wrap/TerrariaServer/ReLogic.dll",
					@"-pre-merge-in=../../../wrap/TerrariaServer/CsvHelper.dll",
					@"-pre-merge-out=../../../TerrariaServer.dll",
					@"-in=../../../TerrariaServer.dll",
#if DEBUG
					@"-mod=../../../OTAPI.Modifications/OTAPI.Modifications.*/bin/Debug/OTAPI.*.dll",
#else
					@"-mod=../../../OTAPI.Modifications/OTAPI.Modifications.*/bin/Release/OTAPI.*.dll",
#endif
					@"-o=../../../OTAPI.dll"
				};
#endif
			}

			options = new OptionSet();
			options.Add("in=|source=", "specifies the source assembly to patch",
				op => sourceAsm = op);
			options.Add("mod=|modifications=", "Glob specifying the path to modification assemblies that will run against the target assembly.",
				op => modificationGlobs.Add(op));
			options.Add("o=|output=", "Specifies the output assembly that has had all modifications applied.",
				op => outputPath = op);
			options.Add("pre-merge-in=", "Specifies an assembly to be combined before any modifications are applied",
				op => mergeInputs.Add(op));
			options.Add("pre-merge-out=", "Specifies the output file of combined assemblies before any modifications are applied",
				op => mergeOutput = op);

			options.Parse(args);

			modificationGlobs.RemoveAll(x => string.IsNullOrEmpty(x));

			if (String.IsNullOrEmpty(sourceAsm) == true
				|| !modificationGlobs.Any())
			{
				options.WriteOptionDescriptions(Console.Out);
				return;
			}

			patcher = new Engine.Patcher(sourceAsm, modificationGlobs, outputPath);

			if (mergeInputs.Count > 0)
			{
				var extractedReferences = new List<String>();
				// extract embedded resources so that ilrepack can find additional references
				foreach (var input in mergeInputs)
				{
					var info = new FileInfo(input);
					if (info.Exists)
					{
						var ext = new Engine.Framework.EmbeddedResourceExtractor(input)
						{
							Extensions = new[] { ".dll", ".exe" }
						};
						extractedReferences.AddRange(ext.Extract());
					}
				}
				// rename resources to match their assembly names
				foreach (var input in extractedReferences)
				{
					var asm = Assembly.LoadFrom(input);
					var dest = Path.Combine(Path.GetDirectoryName(input), asm.GetName().Name + Path.GetExtension(input));

					if (File.Exists(dest))
						File.Delete(dest);

					File.Move(input, dest);

					patcher.AddReference(dest);
				}

				// shims

#if DEBUG
				var path_shims = "../../../OTAPI.Modifications/OTAPI.Modifications.Xna/bin/Debug/OTAPI.Modifications.Xna.dll";
#else
			var path_shims = "../../../OTAPI.Modifications/OTAPI.Modifications.Xna/bin/Release/OTAPI.Modifications.Xna.dll";
#endif
				mergeInputs.Add(path_shims);
				var asm_shims = patcher.AddReference(path_shims);
				for (var i = 0; i < mergeInputs.Count(); i++)
				{
					var input = mergeInputs.ElementAt(i);
					var asm = patcher.AddReference(input);

					var xnaFramework = asm.MainModule.AssemblyReferences
						.Where(x => x.Name.StartsWith("Microsoft.Xna.Framework"))
						.ToArray();

					for (var x = 0; x < xnaFramework.Length; x++)
					{
						xnaFramework[x].Name = "OTAPI.Modifications.Xna"; //TODO: Fix me, ILRepack is adding .dll to the asm name      Context.OTAPI.Assembly.Name.Name;
						xnaFramework[x].PublicKey = asm_shims.Name.PublicKey;
						xnaFramework[x].PublicKeyToken = asm_shims.Name.PublicKeyToken;
						xnaFramework[x].Version = asm_shims.Name.Version;
					}

					var outputFile = $"{input}.shim";

					if (File.Exists(outputFile))
						File.Delete(outputFile);

					asm.Write(outputFile);

					mergeInputs[i] = outputFile;
				}

				var roptions = new ILRepacking.RepackOptions()
				{
					//Get the list of input assemblies for merging
					InputAssemblies = mergeInputs.ToArray(),

					OutputFile = mergeOutput,
					TargetKind = ILRepacking.ILRepack.Kind.Dll,

					//Setup where ILRepack can look for assemblies
					SearchDirectories = mergeInputs
						.Select(x => Path.GetDirectoryName(x))
						.Concat(new[] { Environment.CurrentDirectory })
						.Distinct()
						.ToArray(),

					Parallel = true,
					CopyAttributes = true,
					XmlDocumentation = true,
					UnionMerge = true,

					DebugInfo = true
				};


				Console.WriteLine($"Saving premerge run as {mergeOutput}...");
				var repacker = new ILRepacking.ILRepack(roptions);
				repacker.Repack();
			}

			patcher.Run();
		}
	}
}
