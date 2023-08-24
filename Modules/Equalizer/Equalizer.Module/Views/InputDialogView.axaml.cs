using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Equalizer.Module.Views
{
    public partial class InputDialogView :UserControl
    {
        public InputDialogView()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}