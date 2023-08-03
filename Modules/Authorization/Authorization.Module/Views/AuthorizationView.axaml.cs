using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Authorization.Module.Views
{
    public class AuthorizationView : UserControl
    {
        public AuthorizationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}