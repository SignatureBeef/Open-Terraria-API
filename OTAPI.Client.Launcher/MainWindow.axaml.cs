/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis;
using OTAPI.Client.Launcher.Targets;
using OTAPI.Common;
using OTAPI.Patcher.Targets;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Launcher
{
    public partial class MainWindow : Window
    {
        private FileSystemWatcher _watcher;

        MainWindowViewModel Context { get; set; } = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            IPlatformTarget? target = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                target = new WindowsPlatformTarget();

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                target = new LinuxPlatformTarget();

            else //if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                target = new MacOSPlatformTarget();

            //else throw new NotSupportedException();

            Context.LaunchTarget = target;
            target.OnUILoad(Context);

            DataContext = Context;

            _watcher = new FileSystemWatcher(Environment.CurrentDirectory, "OTAPI.exe");
            _watcher.Created += OTAPI_Changed;
            _watcher.Changed += OTAPI_Changed;
            _watcher.Deleted += OTAPI_Changed;
            _watcher.Renamed += OTAPI_Changed;
            _watcher.EnableRaisingEvents = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _watcher?.Dispose();
            _watcher = null;
        }
        private void OTAPI_Changed(object sender, FileSystemEventArgs e)
        {
            Context.LaunchTarget?.OnUILoad(Context);
        }

        public void OnStartVanilla(object sender, RoutedEventArgs e)
        {
            Program.LaunchID = "VANILLA";
            Program.LaunchFolder = Context.InstallPath.Path;
            this.Close();
        }

        public void OnStartOTAPI(object sender, RoutedEventArgs e)
        {
            Program.LaunchID = "OTAPI";
            Program.LaunchFolder = Context.InstallPath.Path;
            this.Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var target = ClientHelpers.DetermineClientInstallPath(Program.Targets);
            OnInstallPathChange(target);
        }

        void OnInstallPathChange(ClientInstallPath<IPlatformTarget> target)
        {
            Context.InstallPath = target;
            Context.InstallPathValid = target?.Target?.IsValidInstallPath(target.Path) == true;

            target?.Target?.OnUILoad(Context);

            // try copy the icons from the vanilla installation
            try
            {
                if (Context.InstallPathValid)
                {
                    var is_bundle = Path.GetFileName(Environment.CurrentDirectory).Equals("MacOS", StringComparison.CurrentCultureIgnoreCase);
                    if (is_bundle)
                    {
                        var icon_src = Path.Combine(Context.InstallPath.Path, "Resources", "Terraria.icns");
                        var icon_dst = Path.Combine(Environment.CurrentDirectory, "..", "Resources", "OTAPI.icns");

                        if (File.Exists(icon_dst)) File.Delete(icon_dst);

                        if (File.Exists(icon_src) && !File.Exists(icon_dst))
                            File.Copy(icon_src, icon_dst);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to copy icons");
                Console.WriteLine(ex);
            }
        }

        public async void OnFindExe(object sender, RoutedEventArgs e)
        {
            Context.InstallStatus = null;

            var fd = new OpenFolderDialog()
            {
                Directory = Context.InstallPath?.Path
            };
            await fd.ShowAsync(this);

            foreach (var target in Program.Targets)
            {
                if (target.IsValidInstallPath(fd.Directory))
                {
                    Context.InstallPath = new ClientInstallPath<IPlatformTarget>()
                    {
                        Path = fd.Directory,
                        Target = target,
                    };
                    OnInstallPathChange(Context.InstallPath);
                    return;
                }
            }

            Context.InstallStatus = "Install path is not supported";
        }

        public void OnOpenInstallFolder(object sender, RoutedEventArgs e)
        {
            using var process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Environment.CurrentDirectory;
            process.Start();
        }

        public void OnInstall(object sender, RoutedEventArgs e)
        {
            if (Context.IsInstalling) return;
            Context.IsInstalling = true;

            new System.Threading.Thread(() =>
            {
                void CompileCtx(object instance, ModFramework.Modules.CSharp.CSharpLoader.CompilationContextArgs args)
                {
                    var asms = System.AppDomain.CurrentDomain.GetAssemblies();
                    if (args.CoreLibAssemblies is not null && args.CoreLibAssemblies.Count() == 0)
                    {
                        Context.InstallStatus = $"Binding {asms.Length} assemblies";
                        //Console.WriteLine(Context.InstallStatus = $"Binding {asms.Length} assemblies");

                        foreach (var asm in asms.Where(x => !x.IsDynamic
                            && !Path.GetFileName(x.Location).Equals("OTAPI.Patcher.dll"))
                        )
                        {
                            if (!string.IsNullOrWhiteSpace(asm.Location) && File.Exists(asm.Location))
                            {
                                //Console.WriteLine($"Linking {asm.Location}");
                                args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile(asm.Location));
                            }
                        }

                        foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "Syste*.dll"))
                            args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile(file));

                        args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile("netstandard.dll"));
                        args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile("mscorlib.dll"));
                    }
                };
                ModFramework.Modules.CSharp.CSharpLoader.OnCompilationContext += CompileCtx;
                try
                {

                    var target = new OTAPIClientLightweightTarget();

                    target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
                    target.Patch();
                    Context.InstallStatus = "Patching completed, installing to existing installation...";

                    Context.InstallPath.Target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
                    Context.InstallPath.Target.Install(Context.InstallPath.Path);

                    Context.InstallStatus = "Install completed";

                    Context.IsInstalling = false;
                    Context.LaunchTarget.OnUILoad(Context);
                }
                catch (System.Exception ex)
                {
                    Context.InstallStatus = "Err: " + ex.ToString();
                }
                finally
                {
                    ModFramework.Modules.CSharp.CSharpLoader.OnCompilationContext -= CompileCtx;
                }
            }).Start();
        }
    }
}
