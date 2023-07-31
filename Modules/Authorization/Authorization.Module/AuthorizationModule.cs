using System.Resources;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Authorization.Module
{
    /// <summary>
    /// Модуль B
    /// </summary>
    public class AuthorizationModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public AuthorizationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Регистрируем View для навигации по Регионам
            //containerRegistry.RegisterForNavigation<MainView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            //containerProvider.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));

            // Зарегистрировать View к региону.Теперь при запуске ПО View будет привязано сразу
            //_regionManager.RequestNavigate(RegionNameService.ContentRegionName, nameof(MainView));
        }
    }
}
