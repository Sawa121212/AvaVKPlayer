using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaVKPlayer.Views
{
    public partial class EQWindow : UserControl
    {
        public EQWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}