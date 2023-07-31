using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaVKPlayer.ETC
{
    public class LoadingControl : UserControl
    {
        public LoadingControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}