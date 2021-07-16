using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OTAPI.Client.Installer.Targets;
using OTAPI.Common;
using OTAPI.Patcher.Targets;
using ReactiveUI;
using System.Threading.Tasks;

namespace OTAPI.Client.Installer
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ClientInstallPath<IInstallTarget> _installPath;
        public ClientInstallPath<IInstallTarget> InstallPath { get => _installPath; set => this.RaiseAndSetIfChanged(ref _installPath, value); }

        private bool _installPathValid;
        public bool InstallPathValid { get => _installPathValid; set => this.RaiseAndSetIfChanged(ref _installPathValid, value); }

        private string _installStatus;
        public string InstallStatus { get => _installStatus; set => this.RaiseAndSetIfChanged(ref _installStatus, value); }

        private bool _installing;
        public bool IsInstalling { get => _installing; set => this.RaiseAndSetIfChanged(ref _installing, value); }
    }

    public partial class MainWindow : Window
    {
        public MainWindowViewModel Context { get; set; } = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = Context;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var target = ClientHelpers.DetermineClientInstallPath(Program.Targets);
            OnInstallPathChange(target);
        }

        void OnInstallPathChange(ClientInstallPath<IInstallTarget> target)
        {
            Context.InstallPath = target;
            Context.InstallPathValid = target?.Target?.IsValidInstallPath(target.Path) == true;
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
                    Context.InstallPath = new ClientInstallPath<IInstallTarget>()
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

        public void Install(object sender, RoutedEventArgs e)
        {
            if (Context.IsInstalling) return;
            Context.IsInstalling = true;

            Task.Run(() =>
            {
                var target = new OTAPIClientLightweightTarget();
                target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
                target.Patch();
                Context.InstallStatus = "Patching completed, installing to existing installation...";

                Context.InstallPath.Target.StatusUpdate += (sender, e) => Context.InstallStatus = e.Text;
                Context.InstallPath.Target.Install(Context.InstallPath.Path);

                Context.InstallStatus = "Install completed";

                Context.IsInstalling = false;
            });
        }
    }
}
