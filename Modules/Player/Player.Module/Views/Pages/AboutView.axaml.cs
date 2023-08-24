using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VkPlayer.Module.Views.Pages
{
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
