using Authorization.Module.Services;
using Avalonia.Input;
using Player.Domain;
using Player.Domain.ETC;
using Prism.Regions;
using VkNet.Model;
using VkNet.Utils;
using VkProvider.Module;

namespace Player.Module.ViewModels.Audios.Albums
{
    public class OpenAlbumViewModel : AlbumsViewModel, INavigationAware
    {
        public OpenAlbumViewModel(IAuthorizationService authorizationService) : base(authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            AudioAlbumModel? item = args?.GetContent<AudioAlbumModel>();
            if (item == null)
                return;

            MusicFromAlbumViewModel = new MusicFromAlbumViewModel(item);
            MusicFromAlbumViewModel.StartLoad();
            MusicFromAlbumIsVisible = true;
        }

        protected override void LoadData()
        {
            if (_authorizationService.CurrentAccount?.UserId == null)
                return;

            VkCollection<AudioPlaylist>? res = VkApiManager.GetAudioPlaylists(
                (long) _authorizationService.CurrentAccount.UserId, 200,
                (uint) Offset);

            if (res == null)
                return;

            DataCollection.AddRange(res);
            DataCollection.StartLoadImagesAsync();
        }

        /// <inheritdoc />
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        /// <inheritdoc />
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <inheritdoc />
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StartLoad();
        }
    }
}