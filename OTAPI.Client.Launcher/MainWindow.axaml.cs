using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Launcher
{
    interface ILaunchTarget
    {
        void Load(MainWindowViewModel vm);
        void Launch(bool vanilla);
    }

    class OsxLaunch : ILaunchTarget
    {
        public void Load(MainWindowViewModel vm)
        {
            vm.OtapiExe = Path.Combine(Environment.CurrentDirectory, "..", "otapi", "Terraria"); // game host
            vm.VanillaExe = Path.Combine(Environment.CurrentDirectory, "..", "Resources", "Terraria.exe");
        }

        public void Launch(bool vanilla)
        {
            // returns code that correspond to osx ./Terraria launch script
            if (vanilla)
            {
                Environment.Exit(210);
            }
            else
            {
                Environment.Exit(200);
            }
        }
    }

    class WindowsLaunch : ILaunchTarget
    {
        public void Load(MainWindowViewModel vm)
        {
            vm.OtapiExe = Path.Combine(Environment.CurrentDirectory, "otapi", "Terraria.exe"); // game host
            vm.VanillaExe = Path.Combine(Environment.CurrentDirectory, "Terraria.exe");
        }

        public void Launch(bool vanilla)
        {
            if (vanilla)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = "Terraria.orig.exe"
                });
            }
            else
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "otapi"),
                    FileName = "dotnet",
                    Arguments = "Terraria.dll"
                });
            }
        }
    }

    class MainWindowViewModel : ReactiveObject
    {
        private string? _vanillaExe;
        public string? VanillaExe
        {
            get => _vanillaExe;
            set
            {
                IsVanillaReady = File.Exists(value);
                this.RaiseAndSetIfChanged(ref _vanillaExe, value);
            }
        }

        private string? _otapiExe;
        public string? OtapiExe
        {
            get => _otapiExe;
            set
            {
                IsOTAPIReady = File.Exists(value);
                this.RaiseAndSetIfChanged(ref _otapiExe, value);
            }
        }

        private bool _isVanillaReady;
        public bool IsVanillaReady { get => _isVanillaReady; set => this.RaiseAndSetIfChanged(ref _isVanillaReady, value); }

        private bool _isOTAPIReady;
        public bool IsOTAPIReady { get => _isOTAPIReady; set => this.RaiseAndSetIfChanged(ref _isOTAPIReady, value); }

        public ILaunchTarget LaunchTarget { get; set; }

        public void OnStartVanilla()
        {
            LaunchTarget?.Launch(true);
        }

        public void OnStartOTAPI()
        {
            LaunchTarget?.Launch(false);
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var vm = new MainWindowViewModel();
            ILaunchTarget? target = null;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                target = new WindowsLaunch();
            
            else //if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                target = new OsxLaunch();
            
            //else throw new NotSupportedException();
            
            vm.LaunchTarget = target;
            target.Load(vm);

            DataContext = vm;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }
    }
}
