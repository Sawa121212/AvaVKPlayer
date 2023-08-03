using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Common.Resources
{
    public class LoadingControl : UserControl
    {
        public LoadingControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}