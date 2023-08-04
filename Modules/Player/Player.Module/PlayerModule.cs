using System.Resources;
using Common.Core.Localization;
using Common.Core.Regions;
using Player.Module.Properties;
using Player.Module.ViewModels.Audios.Albums;
using Player.Module.ViewModels.Exceptions;
using Player.Module.Views;
using Player.Module.Views.Pages;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Player.Module
{
    public class PlayerModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public PlayerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<AddToAlbumView, AddToAlbumViewModel>();
            containerRegistry.RegisterForNavigation<AlbumListControl>();
            containerRegistry.RegisterForNavigation<ExceptionView, ExceptionViewModel>();
            containerRegistry.RegisterForNavigation<MusicListControl>();
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

            //_regionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(MainView));
        }
    }
}