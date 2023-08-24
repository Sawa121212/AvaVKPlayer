using Equalizer.Module.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Equalizer.Module
{
    public class EqualizerModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(typeof(EqualizerControlView));
            containerRegistry.RegisterInstance(typeof(EqualizerPresetsManagerView));
            containerRegistry.RegisterInstance(typeof(InputDialogView));

            //containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            //containerRegistry.RegisterInstance(typeof(MailViewModel));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}