using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Player.Module.Views
{
    public partial class AddToAlbumView : UserControl
    {
        public AddToAlbumView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}