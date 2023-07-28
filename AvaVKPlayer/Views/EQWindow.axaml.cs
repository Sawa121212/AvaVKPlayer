using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaVKPlayer.Views
{
    public partial class EqWindow : UserControl
    {
        public EqWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}