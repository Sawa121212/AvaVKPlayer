using Prism.Ioc;
using Prism.Modularity;

namespace Equalizer.Module
{
    public class EqualizerModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            //containerRegistry.RegisterInstance(typeof(MailViewModel));
        }
    }
}