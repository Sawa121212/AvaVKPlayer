using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Player.Module.Views
{
    public partial class RepostView : UserControl
    {
        public RepostView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
