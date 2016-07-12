using Mono.Cecil;
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
		protected string modificationAssemblyGlob;

		/// <summary>
		/// Creates a new instance of the patcher with the specified source assembly path and
		/// a glob containing all the modification assemblies.
		/// </summary>
		/// <param name="SourceAssemblyPath"></param>
		public Patcher(string sourceAssemblyPath, string modificationAssemblyGlob, string outputAssemblyPath)
		{
			this.SourceAssemblyPath = sourceAssemblyPath;
			this.modificationAssemblyGlob = modificationAssemblyGlob;
			this.OutputAssemblyPath = outputAssemblyPath;

			resolver = new NugetAssemblyResolver();
			resolver.OnResolved += Resolver_OnResolved;
			readerParams = new ReaderParameters(ReadingMode.Immediate)
			{
				AssemblyResolver = resolver
			};

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			resolver.ResolveFailure += Resolver_ResolveFailure;
		}

		private List<String> resolvedAssemblies = new List<string>();
		private void Resolver_OnResolved(object sender, NugetAssemblyResolvedEventArgs e)
		{
			resolvedAssemblies.Add(e.FilePath);
		}

		private AssemblyDefinition Resolver_ResolveFailure(object sender, AssemblyNameReference reference)
		{
			var assemblyDefinition = this.Modifications.FirstOrDefault(x => x.ModificationDefinition.Name.FullName == reference.FullName);

			if (assemblyDefinition != null)
				return assemblyDefinition.ModificationDefinition;

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

			return null;
		}

		protected void LoadSourceAssembly()
		{
			if (string.IsNullOrEmpty(SourceAssemblyPath))
			{
				throw new ArgumentNullException(nameof(SourceAssemblyPath));
			}

			SourceAssembly = AssemblyDefinition.ReadAssembly(SourceAssemblyPath, readerParams);
		}

		protected IEnumerable<string> GlobModificationAssemblies()
		{
			foreach (var info in Glob.Glob.Expand(modificationAssemblyGlob))
			{
				yield return info.FullName;
			}
		}

		protected ModificationBase LoadModification(Type type)
		{
			object objectRef = Activator.CreateInstance(type);

			return objectRef as ModificationBase;
		}

		protected void LoadModificationAssemblies()
		{
			foreach (string asmPath in GlobModificationAssemblies())
			{
				Assembly asm = null;
				try
				{
					asm = Assembly.LoadFile(asmPath);

					foreach (Type t in asm.GetTypes())
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
								mod.SourceDefinition = SourceAssembly;

								Modifications.Add(mod);
							}
						}
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
			}
		}

		protected void RunModifications()
		{
			foreach (var mod in Modifications.OrderBy(x => x.GetOrder()))
			{
				try
				{
					Console.Write(" -> " + mod.Description);
					mod.Run();
					Console.WriteLine();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Error executing modification {mod.GetType().Name}: {ex.Message}");
					if (System.Diagnostics.Debugger.IsAttached)
						System.Diagnostics.Debugger.Break();
					return;
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
					.Concat(GlobModificationAssemblies())
					//ILRepack will want to find the assemblies we have
					//resolved using our NuGet package resolver or it
					//will crash with not being able to find libraries
					.Concat(resolvedAssemblies)
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

					.Distinct()
					.ToArray(),
				Parallel = true,
				//Version = this.SourceAssembly., //perhaps check an option for this. if changed it should not allow repatching
				CopyAttributes = true,
				XmlDocumentation = true,
				UnionMerge = true,

#if DEBUG
				DebugInfo = true
#endif
			};

			//Generate the allow list of types from our modifications
			AllowDuplcateModificationTypes(options.AllowedDuplicateTypes);

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
		void AllowDuplcateModificationTypes(System.Collections.Hashtable allowedTypes)
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
			if (string.IsNullOrEmpty(OutputAssemblyPath))
			{
				throw new ArgumentNullException(nameof(OutputAssemblyPath));
			}

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

			//Load the ILRepacked assembly so we can find and remove what we need to
			var packedDefinition = AssemblyDefinition.ReadAssembly(tempPackedOutput, new ReaderParameters(ReadingMode.Immediate)
			{
				ReadSymbols = true,
				AssemblyResolver = resolver,
				SymbolReaderProvider = new Mono.Cecil.Pdb.PdbReaderProvider()
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
				WriteSymbols = true,
				SymbolWriterProvider = new Mono.Cecil.Pdb.PdbWriterProvider()
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
				Console.Error.WriteLine($"Error reading source assembly: {ex.Message}");
				return;
			}

			Console.WriteLine($"Patching {SourceAssembly.FullName}.");
			LoadModificationAssemblies();

			Console.WriteLine($"Running {Modifications.Count} modifications.");
			RunModifications();

			Console.WriteLine($"Saving modifications to {OutputAssemblyPath ?? "<null>"}");
			try
			{
				SaveSourceAssembly();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error saving patched source assembly: {ex.Message}");
				return;
			}

			Console.WriteLine("Packing source and modification assemblies.");
			try
			{
				PackAssemblies();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error packing assemblies: {ex.Message}");
				return;
			}

			try
			{
				Console.WriteLine("Cleaning up assembly.");
				RemoveModificationsFromPackedAssembly();
				Cleanup();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error cleaning assembly: {ex.Message}");
				return;
			}
		}
	}
}
