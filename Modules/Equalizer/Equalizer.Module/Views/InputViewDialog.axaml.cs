using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Equalizer.Module.Views
{
    public partial class InputViewDialog :UserControl
    {
        public InputViewDialog()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}