using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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

            //var notifyService = ContainerLocator.Current.Resolve<INotificationService>();
            //notifyService.SetHostWindow(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}