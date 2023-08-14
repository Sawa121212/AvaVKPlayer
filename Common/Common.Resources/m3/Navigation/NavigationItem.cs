using Avalonia.Controls;
using Material.Icons;
using Material.Icons.Avalonia;

namespace Common.Resources.m3.Navigation
{
    public class NavigationItem
    {
        public NavigationItem()
        {
        }

        public NavigationItem(string title) : this()
        {
            Title = title;
        }

        public NavigationItem(MaterialIcon icon) : this()
        {
            Icon = icon;
        }

        public NavigationItem(string title, MaterialIcon icon) : this(title)
        {
            Icon = icon;
        }

        public MaterialIcon Icon { get; set; } = new() {Kind = MaterialIconKind.CircleMedium};

        public string Title { get; set; } = "Item";

        public ContentControl Content { get; set; }
        public object DataContext { get; set; }

        public bool IsEnabled { get; set; } = true;
        public string ToolTip { get; set; }
        public uint Index { get; set; }
    }
}