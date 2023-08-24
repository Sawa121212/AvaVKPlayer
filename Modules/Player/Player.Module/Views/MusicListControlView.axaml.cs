using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VkPlayer.Module.Views
{
    public class MusicListControlView : UserControl
    {
        public MusicListControlView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}