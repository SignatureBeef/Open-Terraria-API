﻿using Mono.Cecil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OTAPI.Patcher.Engine
{
	/// <summary>
	/// Contains the meat of the patcher, organises the source assembly, and a 
	/// list of all the modification instances that will run against the source
	/// assembly.
	/// </summary>
	public class Patcher
	{
		protected class ModAssembly
        {
			public AssemblyDefinition Assembly { get; set; }
			public string Path { get; set; }
		}

		/// <summary>
		/// Gets or sets the path on disk for the source assembly that will have all
		/// the patches run against it.
		/// </summary>
		public string SourceAssemblyPath { get; set; }

		/// <summary>
		/// Gets or sets the path on disk for the output assembly that will be saved
		/// once all modifications have been applied
		/// </summary>
		public string OutputAssemblyPath { get; set; }

		/// <summary>
		/// Gets or sets the assembly definition loaded from the source assembly path.
		/// This assemblydefinition gets modified and rewritten by all the ModificationBase
		/// instances
		/// </summary>
		public AssemblyDefinition SourceAssembly { get; internal set; }

		/// <summary>
		/// Contains a list of all the ModificationBase instances yielded from all the
		/// patch assemblies.
		/// </summary>
		public List<ModificationBase> Modifications { get; set; } = new List<ModificationBase>();

		/// <summary>
		/// Gets or sets the flag to indicate that the source modification should be merged with the modifications
		/// </summary>
		public bool PackModifications { get; set; } = true;

		protected NugetAssemblyResolver resolver;

		protected ReaderParameters readerParams;

		/// <summary>
		/// Temporary storage path for the patched source assembly.
		/// We need it in order to pass it through to ILRepack
		/// </summary>
		private string tempSourceOutput;

		/// <summary>
		/// Temporary storage path for the ILRepacked source assembly.
		/// We need it in order to do cleanups on the final assembly.
		/// </summary>
		private string tempPackedOutput;

		/// <summary>
		/// Glob pattern for the list of modification assemblies that will run against the source
		/// assembly.
		/// </summary>
		protected IEnumerable<string> modificationAssemblyGlob;

		protected List<ModAssembly> modificationAssemblies = new List<ModAssembly>();

		/// <summary>
		/// Creates a new instance of the patcher with the specified source assembly path and
		/// a glob containing all the modification assemblies.
		/// </summary>
		/// <param name="sourceAssemblyPath">Path to the source assembly that modifications need to applied to</param>
		/// <param name="modificationAssemblyGlob">A list of globs that yields a list of modifications</param>
		/// <param name="outputAssemblyPath">Path for the modified assembly to be saved</param>
		public Patcher(string sourceAssemblyPath, IEnumerable<string> modificationAssemblyGlob, string outputAssemblyPath)
		{
			this.SourceAssemblyPath = sourceAssemblyPath;
			this.modificationAssemblyGlob = modificationAssemblyGlob;
			this.OutputAssemblyPath = outputAssemblyPath;

			resolver = new NugetAssemblyResolver();
            resolver.OnResolving += Resolver_OnResolving;
			resolver.OnResolved += Resolver_OnResolved;
			readerParams = new ReaderParameters(ReadingMode.Immediate)
			{
				AssemblyResolver = resolver
			};

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
			resolver.ResolveFailure += Resolver_ResolveFailure;
		}

        private void Resolver_OnResolving(object sender, NugetAssemblyResolvingEventArgs e)
		{
			// try globs instead
			{
				var assemblyDefinition = GetModificationAssemblies().FirstOrDefault(x => x.Assembly.Name.Name == e.Name);
				if (assemblyDefinition?.Assembly != null)
				{
					e.FileLocation = assemblyDefinition.Path;
					return;
				}
			}
		}

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			//Console.WriteLine($"Loaded .net assembly: {args.LoadedAssembly.Location}");
		}

		private List<String> resolvedAssemblies = new List<string>();
		private void Resolver_OnResolved(object sender, NugetAssemblyResolvedEventArgs e)
		{
			//Console.WriteLine($"Resolved nuget assembly: {e.FilePath}");
			resolvedAssemblies.Add(e.FilePath);
		}

		private AssemblyDefinition Resolver_ResolveFailure(object sender, AssemblyNameReference reference)
		{
			var modification = this.Modifications.FirstOrDefault(x => x.ModificationDefinition.Name.FullName == reference.FullName);

			if (modification != null)
			{
				//System.Diagnostics.Debug.WriteLine($"Resolved modification assembly: {reference.FullName}");
				return modification.ModificationDefinition;
			}

			// try globs instead
			{
				var assemblyDefinition = GetModificationAssemblies().FirstOrDefault(x => x.Assembly.Name.FullName == reference.FullName);
				if (assemblyDefinition.Assembly != null)
				{
					return assemblyDefinition.Assembly;
				}
			}
			return null;
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string libPath = Path.GetDirectoryName(args.RequestingAssembly.Location);
			AssemblyName asmName = new AssemblyName(args.Name);

			libPath = Path.Combine(libPath, asmName.Name + ".dll");

			if (File.Exists(libPath))
			{
				return Assembly.LoadFrom(libPath);
			}
			else if (args.Name == SourceAssembly.FullName)
			{
				return Assembly.LoadFrom(SourceAssemblyPath);
			}
			else
			{
				//VS will output artifacts but not dependencies.
				//dotnet however does, and we have now switched to vs as debugging was an issue.
				var modification = GlobModificationAssemblies().SingleOrDefault(x => Path.GetFileName(x) == asmName.Name + ".dll");
				if (modification != null)
				{
					return Assembly.LoadFrom(modification);
				}
				else
				{
					var asm = resolver.Resolve(asmName);
					if (asm != null)
						return asm;
				}
			}

			return null;
		}

		public AssemblyDefinition ReadAssembly(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException(nameof(filePath));
			}

			return AssemblyDefinition.ReadAssembly(filePath, readerParams);
		}

		protected void LoadSourceAssembly()
		{
			SourceAssembly = ReadAssembly(SourceAssemblyPath);
		}

		class LatestMod
		{
			public DateTime LastWriteTimeUtc { get; set; }
			public string FullPath { get; set; }
		}
		
		protected IEnumerable<string> GlobModificationAssemblies()
		{
			var assemblies = new Dictionary<string, LatestMod>();

			foreach (var pattern in modificationAssemblyGlob)
			{
				foreach (var info in Glob.Glob.Expand(pattern))
				{
					if(assemblies.ContainsKey(info.Name))
                    {
						if(assemblies[info.Name].LastWriteTimeUtc < info.LastWriteTimeUtc)
                        {
							assemblies[info.Name] = new LatestMod()
							{
								LastWriteTimeUtc = info.LastWriteTimeUtc,
								FullPath = info.FullName
							};
						}
                    }
					else assemblies.Add(info.Name, new LatestMod()
					{
						LastWriteTimeUtc = info.LastWriteTimeUtc,
						FullPath = info.FullName
					});
				}
			}

			var paths = assemblies.Select(x => x.Value.FullPath).ToArray();
			return paths;
		}

		protected ModificationBase LoadModification(Type type)
		{
			object objectRef = Activator.CreateInstance(type);

			//System.Diagnostics.Debug.WriteLine($"Loaded modification {type.FullName}");

			return objectRef as ModificationBase;
		}

		protected IEnumerable<ModAssembly> GetModificationAssemblies()
        {
			//if (modificationAssemblies == null || modificationAssemblies.Count() == 0)
			//{
			//	var assemblies = new List<(AssemblyDefinition Assembly, string Path)>();
			//	modificationAssemblies = assemblies;

			//	foreach (string asmPath in GlobModificationAssemblies())
			//	{
			//		assemblies.Add(new(ReadAssembly(asmPath), asmPath));
			//	}
			//}

			return modificationAssemblies;
		}

		public AssemblyDefinition AddReference(string path)
		{
			var asm = ReadAssembly(path);

			if (!modificationAssemblies.Any(x => x.Assembly.FullName == asm.FullName))
			{
				modificationAssemblies.Add(new ModAssembly()
				{
					Assembly = asm,
					Path = path
				});
				return asm;
			}

			return asm;
		}

		protected void LoadModificationAssemblies()
		{
			//var assemblies = new List<(AssemblyDefinition Assembly, string Path)>();
			//modificationAssemblies = assemblies;

			foreach (string asmPath in GlobModificationAssemblies())
			{
				Assembly asm = null;
				try
				{
					asm = Assembly.LoadFile(asmPath);

					AddReference(asmPath);

					//assemblies.Add(new(ReadAssembly(asmPath), asmPath));

					var types = asm.GetTypes();
					foreach (Type t in types)
					{
						if (t.BaseType != typeof(ModificationBase))
						{
							continue;
						}

						//Prevent duplicates caused from modifications referencing other modifications (ie core/xna)
						if (!Modifications.Any(m => m.GetType().FullName == t.FullName))
						{
							ModificationBase mod = LoadModification(t);
							if (mod != null)
							{
								//Is the mod applicable to the current source assembly?
								if (mod.AssemblyTargets.Contains(SourceAssembly.Name.FullName))
								{
									mod.SourceDefinition = SourceAssembly;
									mod.SourceDefinitionFilePath = asmPath;

									//Console.WriteLine($"Ready to run modification {t.FullName}");
									Modifications.Add(mod);
								}
								else
								{
									Console.WriteLine($"Modification {t.FullName} does not support {SourceAssembly.Name.FullName}.");
								}
							}
						}
						//else
						//{
						//	Console.WriteLine($"Skipping modification {t.FullName}");
						//}
					}
				}
				catch (ReflectionTypeLoadException rtle)
				{
					Console.Error.WriteLine($" * Error loading modification from {asmPath}, it will be skipped.");
					foreach (var error in rtle.LoaderExceptions)
					{
						Console.Error.WriteLine($" -- > {error.Message}");
					}

					continue;
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($" * Error loading modification from {asmPath}, it will be skipped.\n{ex}");

					continue;
				}
			}
		}

		protected void RunModifications()
		{
			foreach (var mod in Modifications.OrderBy(x => x.GetOrder()))
			{
				try
				{
					//System.Diagnostics.Debug.WriteLine($"Running modification {mod.GetType().FullName} from file {mod.SourceDefinitionFilePath}");
					Console.Write(" -> " + mod.Description);
					mod.Run();
					Console.WriteLine();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Error executing modification {mod.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
					if (System.Diagnostics.Debugger.IsAttached)
						System.Diagnostics.Debugger.Break();
					throw;
				}
			}
		}

		protected void SaveSourceAssembly()
		{
			tempSourceOutput = Path.GetTempFileName() + ".dll";

			this.SourceAssembly.Write(tempSourceOutput, new WriterParameters()
			{
				//WriteSymbols = true
			});
		}

		protected void PackAssemblies()
		{
			tempPackedOutput = Path.GetTempFileName() + ".dll";

			var options = new ILRepacking.RepackOptions()
			{
				//Get the list of input assemblies for merging, ensuring our source 
				//assembly is first in the list so it can be granted as the target assembly.
				InputAssemblies = new[] { tempSourceOutput }
					//Add the modifications for merging
					.Concat(Modifications.Select(x => x.SourceDefinitionFilePath))

					.ToArray(),

				OutputFile = tempPackedOutput,
				TargetKind = ILRepacking.ILRepack.Kind.Dll,

				//Setup where ILRepack can look for assemblies
				SearchDirectories = GlobModificationAssemblies()
					//Translate full path files found via the glob mechanism
					//into a directory name
					.Select(x => Path.GetDirectoryName(x))

					//Additionally we may have resolved libraries using NuGet,
					//so we also append the directory of each assembly as well
					.Concat(resolvedAssemblies.Select(x => Path.GetDirectoryName(x)))

					//ILRepack rolls is own silly cecil version, keeps the namespace and hides
					//custom assembly resolvers. We have to explicitly add in cecil or it wont 
					//be found
					.Concat(new[] { Path.GetDirectoryName(typeof(Mono.Cecil.AssemblyDefinition).Assembly.Location) })

					.Distinct()
					.ToArray(),
				Parallel = true,
				//Version = this.SourceAssembly., //perhaps check an option for this. if changed it should not allow repatching
				CopyAttributes = true,
				XmlDocumentation = true,
				UnionMerge = true,
				
				DebugInfo = true
			};

			//Generate the allow list of types from our modifications
			AllowDuplicateModificationTypes(options.AllowedDuplicateTypes);

			var repacker = new ILRepacking.ILRepack(options);
			repacker.Repack();
		}

		/// <summary>
		/// This will allow ILRepack to merge all our modifications into one assembly.
		/// In our case we define hooks in the same namespace across different modification
		/// assemblies, and of course ILRepack detects the conflict.
		/// By enumerating each type of the modifications, we add each full name to the
		/// allowed list.
		/// </summary>
		/// <param name="allowedTypes"></param>
		void AllowDuplicateModificationTypes(System.Collections.Hashtable allowedTypes)
		{
			foreach (var mod in Modifications)
			{
				mod.ModificationDefinition.MainModule.ForEachType(type =>
				{
					allowedTypes[type.FullName] = type.FullName;
				});
			}
		}

		/// <summary>
		/// Removes all ModificationBase implementations from the output assembly.
		/// Additionally it will also remove references that were added due to ModificationBases
		/// being merged in.
		/// </summary>
		protected void RemoveModificationsFromPackedAssembly()
		{
			var assemblyName = Path.GetFileNameWithoutExtension(OutputAssemblyPath);

			#region Xml Comments
			//ILRepack already constructed the xml comments file for us
			//so all we need to do is copy and correct it's assembly name.
			var srcXml = Path.ChangeExtension(tempPackedOutput, "xml");
			if (File.Exists(srcXml))
			{
				var destXml = Path.ChangeExtension(OutputAssemblyPath, "xml");
				var tempAssemblyName = Path.GetFileNameWithoutExtension(tempPackedOutput);

				var comments = File.ReadAllText(srcXml).Replace(tempAssemblyName, assemblyName);
				File.WriteAllText(destXml, comments);
			}
			#endregion

			var handlePdbs = File.Exists(Path.ChangeExtension(tempPackedOutput, "pdb"));

			//Load the ILRepacked assembly so we can find and remove what we need to
			var packedDefinition = AssemblyDefinition.ReadAssembly(tempPackedOutput, new ReaderParameters(ReadingMode.Immediate)
			{
				ReadSymbols = handlePdbs,
				AssemblyResolver = resolver,
				SymbolReaderProvider = handlePdbs ? new Mono.Cecil.Pdb.PdbReaderProvider() : null
			});

			//Now we enumerate over the references and mainly remove ourself (the engine).
			foreach (var reference in packedDefinition.MainModule.AssemblyReferences
				.Where(x => new[] { /*"Mono.Cecil", "Mono.Cecil.Rocks",*/ typeof(Patcher).Assembly.GetName().Name }.Contains(x.Name))
				.ToArray())
			{
				packedDefinition.MainModule.AssemblyReferences.Remove(reference);
			}

			//Remove all ModificationBase implementations from the output.
			//Normal circumstances this shouldn't matter, but fair chance if 
			//a implementor or their plugin enumerates over each type it will
			//cause issues as in our case we are not shipping cecil.
			foreach (var mod in Modifications)
			{
				//Find the TypeDefinition using the Modification's type name
				var type = packedDefinition.MainModule.Type(mod.GetType().FullName);

				//Remove it from the packed assembly
				packedDefinition.MainModule.Types.Remove(type);
			}

			//We are changing the file name from a temporary file, so the assembly name
			//will also need to reflect this.
			packedDefinition.Name.Name = assemblyName;

			//All modifications and cleaning is complete, we can now save the final assembly.
			packedDefinition.Write(OutputAssemblyPath, new WriterParameters()
			{
				WriteSymbols = handlePdbs,
				SymbolWriterProvider = handlePdbs ? new Mono.Cecil.Pdb.PdbWriterProvider() : null
			});
		}

		/// <summary>
		/// Cleans up data after a successful patch
		/// </summary>
		protected void Cleanup()
		{
			if (File.Exists(tempPackedOutput))
				File.Delete(tempPackedOutput);

			if (File.Exists(tempSourceOutput))
				File.Delete(tempSourceOutput);
		}

		public void Run()
		{
			try
			{
				LoadSourceAssembly();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error reading source assembly: {ex}");
				return;
			}

			Console.WriteLine($"Patching {SourceAssembly.FullName}.");
			LoadModificationAssemblies();

			Console.WriteLine($"Running {Modifications.Count} modifications.");
			RunModifications();

			Console.WriteLine("Saving modifications for merge...");
			try
			{
				SaveSourceAssembly();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error saving patched source assembly: {ex}");
				return;
			}

			if (PackModifications)
			{
				Console.WriteLine("Packing source and modification assemblies.");
				try
				{
					PackAssemblies();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Error packing assemblies: {ex}");
					return;
				}
			}

			try
			{
				Console.WriteLine("Cleaning up assembly.");

				if (string.IsNullOrEmpty(OutputAssemblyPath))
				{
					throw new ArgumentNullException(nameof(OutputAssemblyPath));
				}

				if (PackModifications)
				{
					RemoveModificationsFromPackedAssembly();
				}
				else
				{
					//Since nothing was packed our files need to be manually copied
					//ILRepack would otherwise do this for us
					foreach (var ext in new[]
					{
						"exe",
						"dll",
						"pdb",
						"xml",
						"exe.mdb",
						"dll.mdb"
					})
					{
						var source = Path.ChangeExtension(tempSourceOutput, ext);
						if (!File.Exists(source))
						{
							source = Path.ChangeExtension(SourceAssemblyPath, ext);
						}

						if (File.Exists(source))
						{
							var dest = Path.ChangeExtension(OutputAssemblyPath, ext);

							if (File.Exists(dest))
							{
								File.Delete(dest);
							}
							File.Copy(source, dest);
						}
						//else Console.WriteLine($"Failed to copy {source}");
					}
				}
				Cleanup();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error cleaning assembly: {ex}");
				return;
			}

			Console.WriteLine($"Patching complete. Output: {Path.GetFullPath(OutputAssemblyPath)}");
		}
	}
}
