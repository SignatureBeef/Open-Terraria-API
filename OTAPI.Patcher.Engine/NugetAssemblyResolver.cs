using Mono.Cecil;
using NuGet;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Engine
{
	public class NugetAssemblyResolvedEventArgs : EventArgs
	{
		public string FilePath { get; set; }
	}

	public class NugetAssemblyResolver : DefaultAssemblyResolver
	{
		protected string NugetFeedUri => "https://packages.nuget.org/api/v2";
		protected IPackageRepository packageRepo;
		protected IPackageRepository localPackageRepo;
		protected IPackageManager packageManager;
		protected string packageInstallDir;

		public string PackagesDirectory => packageInstallDir;

		public event EventHandler<NugetAssemblyResolvedEventArgs> OnResolved;

		public NugetAssemblyResolver()
		{
			packageRepo = PackageRepositoryFactory.Default.CreateRepository(NugetFeedUri);

			packageInstallDir = Path.Combine(Path.GetTempPath(), "cecilnuget");
			Directory.CreateDirectory(packageInstallDir);

			packageManager = new PackageManager(packageRepo, packageInstallDir);
			packageManager.PackageInstalled += PackageManager_PackageInstalled;
			localPackageRepo = packageManager.LocalRepository;
		}

		private void PackageManager_PackageInstalled(object sender, PackageOperationEventArgs e)
		{
			Console.WriteLine($" * NuGet: Package {e.Package.GetFullName()} installed to {e.InstallPath}.");
		}

		/// <summary>
		/// Resolves a nuget packege locally or from the internet and automatically installs it.
		/// </summary>
		/// <param name="name">Name of the package to find</param>
		/// <param name="version">Version of the package</param>
		/// <returns>Nuget package details</returns>
		protected IPackage ResolvePackage(string name, SemanticVersion version)
		{
			IPackage package = null;

			package = localPackageRepo.FindPackage(name, version);
			if (package == null)
			{
				package = packageRepo.FindPackage(name, version);
			}

			if (package != null)
			{
				packageManager.InstallPackage(package, ignoreDependencies: true, allowPrereleaseVersions: false);
			}

			return package;
		}

		/// <summary>
		/// Resolves a package from nuget using a specified name and version.
		/// </summary>
		/// <param name="name">Name of the package to find</param>
		/// <param name="version">Version of the package</param>
		/// <returns>File path if the nuget package is found</returns>
		protected string ResolvePackage(string name, Version version)
		{
			//SemanticVersion vers = SemanticVersion.ParseOptionalVersion($"{version.Major}.{version.Minor}.*");
			SemanticVersion vers = SemanticVersion.ParseOptionalVersion($"{version.Major}.{version.Minor}.{version.MajorRevision}.{version.MinorRevision}");
			IPackage package = ResolvePackage(name, vers);

			if (package != null)
			{
				string installPath = packageManager.PathResolver.GetInstallPath(package);
				string libPath = Path.Combine(installPath, "lib", "net45", name + ".dll");

				if (File.Exists(libPath))
				{
					OnResolved?.Invoke(this, new NugetAssemblyResolvedEventArgs()
					{
						FilePath = libPath
					});
					return libPath;
				}
				else
				{
					var files = Directory.EnumerateFiles(installPath, "*.dll", SearchOption.AllDirectories);
					if (files.Count() == 1)
					{
						libPath = files.Single();
						OnResolved?.Invoke(this, new NugetAssemblyResolvedEventArgs()
						{
							FilePath = libPath
						});
						return libPath;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Resolves a assembly from nuget and loads it as a .net assembly
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public System.Reflection.Assembly Resolve(System.Reflection.AssemblyName name)
		{
			var filePath = ResolvePackage(name.Name, name.Version);
			if (filePath != null)
			{
				return System.Reflection.Assembly.LoadFrom(filePath);
			}

			return null;
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			//Don't resolve system files (at least temporarily. i cannot run without internet otherwise).
			if (Array.IndexOf(new[]
			{
				"mscorlib"
			}, name.Name) > -1)
				return base.Resolve(name);

			var filePath = ResolvePackage(name.Name, name.Version);
			if (filePath != null)
			{
				return ModuleDefinition.ReadModule(filePath, new ReaderParameters(ReadingMode.Immediate)).Assembly;
			}

			return base.Resolve(name);
		}
	}
}
