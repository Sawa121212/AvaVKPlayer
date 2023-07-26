using Avalonia.Input;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;

namespace AvaVKPlayer.ViewModels.Audios.Albums
{
    public class OpenAlbumViewModel : AlbumsViewModel
    {
        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            var item = args?.GetContent<AudioAlbumModel>();
            if (item != null)
            {
                MusicFromAlbumViewModel = new MusicFromAlbumViewModel(item);
                MusicFromAlbumViewModel.StartLoad();
                MusicFromAlbumIsVisible = true;
            }
        }

        protected override void LoadData()
        {
            if (GlobalVars.CurrentAccount?.UserID != null)
            {
                var res = GlobalVars.VkApi.Audio.GetPlaylists((long)GlobalVars.CurrentAccount.UserID, 200,
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