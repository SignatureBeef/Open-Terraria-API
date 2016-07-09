using ILRepacking;
using Mono.Cecil;
using NuGet;
using System;
using System.IO;

namespace OTAPI.Patcher.Engine
{
	public class NugetAssemblyResolvedEventArgs : EventArgs
	{
		public AssemblyDefinition Assembly { get; set; }
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

		protected IPackage ResolvePackage(string name, Version version)
		{
			IPackage package = null;
			SemanticVersion vers = SemanticVersion.ParseOptionalVersion($"{version.Major}.{version.Minor}.*");

			package = localPackageRepo.FindPackage(name, vers);
			if (package == null)
			{
				package = packageRepo.FindPackage(name, vers);
			}

			if (package != null)
			{
				packageManager.InstallPackage(package, ignoreDependencies: true, allowPrereleaseVersions: false);
			}

			return package;
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			IPackage package = ResolvePackage(name.Name, name.Version);

			if (package != null)
			{
				string libPath = Path.Combine(packageManager.PathResolver.GetInstallPath(package),
					"lib", "net45", name.Name + ".dll");

				if (File.Exists(libPath))
				{
					AssemblyDefinition asmdef = ModuleDefinition.ReadModule(libPath, new ReaderParameters(ReadingMode.Immediate)).Assembly;
					OnResolved?.Invoke(this, new NugetAssemblyResolvedEventArgs()
					{
						Assembly = asmdef,
						FilePath = libPath
					});
					return asmdef;
				}
			}

			return base.Resolve(name);
		}
	}
}
