using Avalonia.Input;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels.Audios.Albums
{
    public class OpenAlbumViewModel : AlbumsViewModel
    {
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