using Authorization.Module.Services;
using Avalonia.Input;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels.Audios.Albums
{
    /// <summary>
    /// Музыка из альбома
    /// </summary>
    public class OpenAlbumViewModel : AlbumsViewModel
    {
        public OpenAlbumViewModel(IAuthorizationService authorizationService) : base(authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <inheritdoc/>
        public override void OnSelectedItem(object sender, PointerPressedEventArgs args)
        {
            AudioAlbumModel? item = args?.GetContent<AudioAlbumModel>();
            if (item == null)
                return;

            MusicFromAlbumViewModel = new MusicFromAlbumViewModel(item);
            MusicFromAlbumViewModel.StartLoad();
            IsVisibleMusicFromAlbum = true;
        }

        /// <inheritdoc/>
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

        protected readonly IAuthorizationService _authorizationService;

    }
}