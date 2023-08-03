using Prism.Events;
using Prism.Regions;

namespace Authorization.Module.Events
{
    public class UserControlViewChangedEvent : PubSubEvent<UserControlViewChangedEvent>
    {
        public UserControlViewChangedEvent()
        {
        }

        public UserControlViewChangedEvent(string viewName)
        {
            ViewName = viewName;
        }

        public string ViewName { get; }
        public NavigationParameters NavigationParameters { set; get; }
    }
}