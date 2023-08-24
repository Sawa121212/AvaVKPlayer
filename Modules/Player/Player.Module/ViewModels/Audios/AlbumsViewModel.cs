using Authorization.Module.Services;
using Avalonia.Input;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.Base;
using VkPlayer.Domain.ETC;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels.Audios
{
    public class AlbumsViewModel : DataViewModelBase<AudioAlbumModel>
    {
        public AlbumsViewModel(IAuthorizationService authorizationService) : base()
        {
            _authorizationService = authorizationService;
            BackToAlbumListCommand = ReactiveCommand.Create(() =>
            {
                IsVisibleMusicFromAlbum = false;
                MusicFromAlbumViewModel = null;
                SelectedIndex = -1;
            });
        }

        /// <inheritdoc />
        public override void OnSelected(AudioAlbumModel albumModel)
        {
            if (albumModel == null)
                return;

            MusicFromAlbumViewModel = new MusicFromAlbumViewModel(albumModel);
            MusicFromAlbumViewModel.StartLoad();
            IsVisibleMusicFromAlbum = true;
        }

        /// <inheritdoc />
        public override void OnSelectedItem(object sender, PointerPressedEventArgs args)
        {
            AudioAlbumModel? item = args?.GetContent<AudioAlbumModel>();
            if (item == null)
                return;

            MusicFromAlbumViewModel = new MusicFromAlbumViewModel(item);
            MusicFromAlbumViewModel.StartLoad();
            IsVisibleMusicFromAlbum = true;
        }

        /// <inheritdoc />
        protected override void LoadData()
        {
            if (_authorizationService.CurrentAccount?.UserId == null)
            {
                return;
            }

            VkCollection<AudioPlaylist>? res = VkApiManager.GetAudioPlaylists(
                (long) _authorizationService.CurrentAccount.UserId, 200,
                (uint) Offset);

            if (res == null)
                return;

            DataCollection.AddRange(res);

            DataCollection.StartLoadImagesAsync();
        }


        public MusicFromAlbumViewModel? MusicFromAlbumViewModel { get; set; }

        public bool IsVisibleMusicFromAlbum { get; set; }

        public IReactiveCommand BackToAlbumListCommand { get; }

        protected readonly IAuthorizationService _authorizationService;
    }
}