using System.Resources;
using Common.Core.Localization;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using VkPlayer.Module.Properties;
using VkPlayer.Module.ViewModels.Audios.Albums;
using VkPlayer.Module.ViewModels.Exceptions;
using VkPlayer.Module.Views;
using VkPlayer.Module.Views.Pages;

namespace VkPlayer.Module
{
    public class VkPlayerModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public VkPlayerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<AddToAlbumView, AddToAlbumViewModel>();
            containerRegistry.RegisterForNavigation<AlbumListControl>();
            containerRegistry.RegisterForNavigation<ExceptionView, ExceptionViewModel>();
            containerRegistry.RegisterForNavigation<MusicListControlView>();
            containerRegistry.RegisterForNavigation<PlayerControlView, PlayerControlViewModel>();
            containerRegistry.RegisterForNavigation<RepostView, RepostViewModel>();

            // Pages
            containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            containerProvider.Resolve<ILocalizer>().AddResourceManager(new ResourceManager(typeof(Language)));
        }
    }
}