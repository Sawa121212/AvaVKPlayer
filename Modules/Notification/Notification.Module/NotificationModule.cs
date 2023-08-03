using Notification.Module.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace Notification.Module
{
    public class NotificationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            //containerRegistry.RegisterInstance(typeof(MailViewModel));
        }
    }
}