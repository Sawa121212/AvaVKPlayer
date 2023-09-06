using System.Resources;
using Authorization.Module.Properties;
using Authorization.Module.Services;
using Authorization.Module.Views;
using Common.Core.Extensions;
using Common.Core.Localization;
using Prism.Ioc;
using Prism.Modularity;

namespace Authorization.Module
{
    /// <summary>
    /// Модуль авторизации
    /// </summary>
    public class AuthorizationModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.TryRegisterSingleton<IAuthorizationService, AuthorizationService>();

            // Регистрируем View для навигации по Регионам
            containerRegistry.TryRegisterForNavigation<AuthorizationView, AuthorizationViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            containerProvider.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));
        }
    }
}