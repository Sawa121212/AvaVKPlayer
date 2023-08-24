using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VkPlayer.Module.Views.Pages
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
