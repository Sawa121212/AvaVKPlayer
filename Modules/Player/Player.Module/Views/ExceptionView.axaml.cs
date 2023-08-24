using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VkPlayer.Module.Views
{
    public class ExceptionView : UserControl
    {
        public ExceptionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}