using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Equalizer.Module.Views
{
    public partial class EqualizerPresetsManager : UserControl
    {
        public EqualizerPresetsManager()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}