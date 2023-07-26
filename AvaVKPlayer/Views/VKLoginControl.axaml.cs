using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaVKPlayer.Views
{
    public class VKLoginControl : UserControl
    {
        public VKLoginControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}