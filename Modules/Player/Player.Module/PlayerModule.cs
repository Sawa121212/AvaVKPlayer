using Player.Module.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Player.Module
{
    public class PlayerModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            containerRegistry.RegisterForNavigation<MainView>();
            containerRegistry.RegisterForNavigation<AddToAlbumView>();
            containerRegistry.RegisterForNavigation<AlbumListControl>();
            containerRegistry.RegisterForNavigation<ExceptionView>();
            containerRegistry.RegisterForNavigation<MusicListControl>();
            containerRegistry.RegisterForNavigation<PlayerControlView>();
            containerRegistry.RegisterForNavigation<RepostView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}