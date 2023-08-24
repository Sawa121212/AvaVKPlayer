using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Equalizer.Module.Views
{
    public partial class EqualizerPresetsManagerView : UserControl
    {
        public EqualizerPresetsManagerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}