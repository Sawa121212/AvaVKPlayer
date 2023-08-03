using Authorization.Module.Domain;
using Common.Core.ToDo;
using Player.Domain.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;

namespace Player.Domain
{
    public class AudioModel : VkAudioOrAlbumModelBase
    {
        private int _downloadPercent;
        private bool _isDownload;

        public AudioModel()
        {
            Title = "Название";
            Artist = "Исполнитель";
            Image = new ImageModel
            {
                DecodeWidth = 50,
                Bitmap = GlobalVars.DefaultMusicImage
            };
        }

        [Reactive] public LyricsViewModel? LyricsViewModel { get; set; }


        public bool LyricsButtonIsVisible
        {
            get => false;
        }

        public AudioModel(Audio vkModel) : this()
        {
            IsNotAvailable = vkModel?.ContentRestricted != null;

            AccessKey = vkModel.AccessKey;
            if (vkModel.LyricsId != null)
            {
                LyricsViewModel = new LyricsViewModel(vkModel.LyricsId);
            }

            Duration = vkModel.Duration;
            Id = (long) vkModel.Id;
            OwnerId = (long) vkModel.OwnerId;
            Artist = vkModel.Artist;
            Title = vkModel.Title;
            Subtitle = vkModel.Subtitle;

            if (vkModel.Album != null && vkModel.Album.Thumb != null)
                Image.ImageUrl = GetThumbUrl(vkModel.Album.Thumb);
        }

        public int DownloadPercent
        {
            get => _downloadPercent;
            set => this.RaiseAndSetIfChanged(ref _downloadPercent, value);
        }

        public bool IsDownload
        {
            get => _isDownload;
            set
            {
                if (value == false)
                    DownloadPercent = 0;

                this.RaiseAndSetIfChanged(ref _isDownload, value);
            }
        }


        public int Duration { get; set; }


        public override string GetThumbUrl(AudioCover audioCover)
        {
            if (audioCover.Photo68 != null)
                return audioCover.Photo68;

            if (audioCover.Photo135 != null)
                return audioCover.Photo135;

            return string.Empty;
        }
    }
}