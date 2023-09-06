using System.Resources;
using Common.Core.Extensions;
using Common.Core.Localization;
using Notification.Module.Properties;
using Notification.Module.Services;
using Notification.Module.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Notification.Module
{
    public class NotificationModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.TryRegisterSingleton<INotificationService, NotificationService>();
            containerRegistry.TryRegister<NoticeDialogView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            containerProvider.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));
        }
    }
}