using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VkPlayer.Module.Views
{
    public class AlbumListControl : UserControl
    {
        public AlbumListControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}