using System.Collections.Generic;

namespace Common.Resources.m3.Navigation
{
    public class NavigationMenu
    {
        public NavigationMenu()
        {
            ItemSource = new List<NavigationItem>();
        }

        public void AddItem(NavigationItem item)
        {
            if (item != null)
            {
                ItemSource.Add(item);
            }
        }

        public List<NavigationItem> ItemSource { get; private set; }
    }
}