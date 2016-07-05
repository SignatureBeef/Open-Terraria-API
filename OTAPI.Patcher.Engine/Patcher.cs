using Mono.Cecil;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Glob;
using System.IO;

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

		protected ReaderParameters readerParams = new ReaderParameters(ReadingMode.Immediate)
		{
			AssemblyResolver = new NugetAssemblyResolver()
		};


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

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
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

						ModificationBase mod = LoadModification(t);
						if (mod != null)
						{
							mod.SourceDefinition = SourceAssembly;

							Modifications.Add(mod);
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
			if (string.IsNullOrEmpty(OutputAssemblyPath))
			{
				throw new ArgumentNullException(nameof(OutputAssemblyPath));
			}

			this.SourceAssembly.Write(OutputAssemblyPath);
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

			SaveSourceAssembly();
		}
	}
}
