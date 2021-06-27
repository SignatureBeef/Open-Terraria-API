using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Launcher
{
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

        public void OnStartVanilla()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = "Terraria.orig.exe"
            });
        }

        public void OnStartOTAPI()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "otapi"),
                FileName = "dotnet",
                Arguments = "Terraria.dll"
            });
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

            vm.OtapiExe = Path.Combine(Environment.CurrentDirectory, "otapi", "Terraria.exe"); // game host

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                vm.VanillaExe = Path.Combine(Environment.CurrentDirectory, "Terraria.exe");

            DataContext = vm;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
