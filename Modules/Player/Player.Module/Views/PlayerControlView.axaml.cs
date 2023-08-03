using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Player.Module.Views
{
    public sealed class PlayerControlView : UserControl
    {
        public PlayerControlView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}