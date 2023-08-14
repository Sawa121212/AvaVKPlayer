using Authorization.Module.Services;
using Avalonia.Input;
using Player.Domain;
using Player.Domain.Base;
using Player.Domain.ETC;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;
using VkNet.Utils;
using VkProvider.Module;

namespace Player.Module.ViewModels.Audios
{
    public class AlbumsViewModel : DataViewModelBase<AudioAlbumModel>
    {
        internal IAuthorizationService _authorizationService;

        public AlbumsViewModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            BackToAlbumListCommand = ReactiveCommand.Create(() =>
            {
                MusicFromAlbumIsVisible = false;
                MusicFromAlbumViewModel = null;
                SelectedIndex = -1;
            });
        }

        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            AudioAlbumModel? item = args?.GetContent<AudioAlbumModel>();
            if (item != null)
            {
                MusicFromAlbumViewModel = new MusicFromAlbumViewModel(item);
                MusicFromAlbumViewModel.StartLoad();
                MusicFromAlbumIsVisible = true;
            }
        }


        protected override void LoadData()
        {
            if (_authorizationService.CurrentAccount?.UserId != null)
            {
                VkCollection<AudioPlaylist>? res = VkApiManager.GetAudioPlaylists(
                    (long) _authorizationService.CurrentAccount.UserId, 200,
                    (uint) Offset);

                if (res == null)
                    return;

                DataCollection.AddRange(res);

                DataCollection.StartLoadImagesAsync();
            }
        }


        [Reactive] public MusicFromAlbumViewModel? MusicFromAlbumViewModel { get; set; }

        [Reactive] public bool MusicFromAlbumIsVisible { get; set; }

        public IReactiveCommand BackToAlbumListCommand { get; set; }
    }
}