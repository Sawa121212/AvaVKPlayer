using System;
using Avalonia.Layout;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.ViewModels.Base;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels.Audios
{
    public sealed class MusicFromAlbumViewModel : AudioViewModelBase
    {
        public MusicFromAlbumViewModel(AudioAlbumModel audioAlbumModel)
        {
            Album = audioAlbumModel;

            StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            Events.AudioRemoveFromAlbumEvent += MusicFromAlbumViewModel_AudioRemoveEvent;

            if (Album.OwnerId == GlobalVars.VkApi.UserId && !Album.IsFollowing)
                AudioListButtons.AudioAddIsVisible = false;
            else
                AudioListButtons.AudioRemoveIsVisible = false;
            AudioListButtons.Album = Album;
        }

        private AudioAlbumModel Album { get; }

        private void MusicFromAlbumViewModel_AudioRemoveEvent(AudioModel audioModel) =>
            AllDataCollection?.Remove(audioModel);


        protected override void LoadData()
        {
            VkCollection<Audio>? res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
            {
                Count = 500,
                Offset = (uint) Offset,
                PlaylistId = Album.Id
            });
            if (res != null)
            {
                DataCollection.AddRange(res);
                ResponseCount = res.Count;

                DataCollection.StartLoadImagesAsync();
                Offset += res.Count;
            }

            AllDataCollection = DataCollection;
        }
    }
}