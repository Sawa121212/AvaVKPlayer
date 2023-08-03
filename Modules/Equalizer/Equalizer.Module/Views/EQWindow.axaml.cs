using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Equalizer.Module.Views
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