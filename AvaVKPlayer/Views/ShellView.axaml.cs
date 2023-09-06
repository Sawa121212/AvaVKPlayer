using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Notification.Module.Services;
using Prism.Ioc;

namespace AvaVKPlayer.Views
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            // Initialize the WindowNotificationManager with the MainWindow
            INotificationService? notifyService = ContainerLocator.Current.Resolve<INotificationService>();
            notifyService.SetHostWindow(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}