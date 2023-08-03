using System;
using Avalonia.Layout;
using Player.Domain;
using Player.Domain.ETC;
using Player.Module.ViewModels.Base;
using VkNet.Model;
using VkNet.Utils;
using VkProvider.Module;

namespace Player.Module.ViewModels.Audios
{
    public sealed class MusicFromAlbumViewModel : AudioViewModelBase
    {
        public MusicFromAlbumViewModel(AudioAlbumModel audioAlbumModel)
        {
            Album = audioAlbumModel;

            StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            // ToDo Events.AudioRemoveFromAlbumEvent += MusicFromAlbumViewModel_AudioRemoveEvent;

            if (Album.OwnerId == VkApiManager.VkApi.UserId && !Album.IsFollowing)
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
            VkCollection<Audio>? res = VkApiManager.GetAudio(new AudioGetParams
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