using Avalonia.Input;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.ViewModels.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels.Audios
{
    public class AlbumsViewModel : DataViewModelBase<AudioAlbumModel>
    {

        public AlbumsViewModel()
        {
            BackToAlbumListCommand = ReactiveCommand.Create(() =>
            {
                MusicFromAlbumIsVisible = false;
                MusicFromAlbumViewModel = null;
                SelectedIndex = -1;
            });
        }

        public IReactiveCommand BackToAlbumListCommand { get; set; }

        [Reactive]
        public MusicFromAlbumViewModel? MusicFromAlbumViewModel { get; set; }

        [Reactive]
        public bool MusicFromAlbumIsVisible { get; set; }


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
            if (GlobalVars.CurrentAccount?.UserId != null)
            {
                VkCollection<AudioPlaylist>? res = GlobalVars.VkApi.Audio.GetPlaylists((long)GlobalVars.CurrentAccount.UserId, 200,
                    (uint)Offset);
                if (res != null)
                {
                    DataCollection.AddRange(res);

                    DataCollection.StartLoadImagesAsync();
                }
            }
        }
    }
}