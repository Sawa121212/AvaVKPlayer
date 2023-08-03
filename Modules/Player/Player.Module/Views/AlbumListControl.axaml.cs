using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Player.Module.Views
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