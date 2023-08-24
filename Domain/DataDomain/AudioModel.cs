using Authorization.Module.Domain;
using ReactiveUI;
using VkNet.Model;
using VkPlayer.Domain.Base;

namespace VkPlayer.Domain
{
    /// <summary>
    /// Модель музыки
    /// </summary>
    public class AudioModel : VkAudioOrAlbumModelBase
    {
        private int _downloadPercent;
        private bool _isDownload;
        private LyricsViewModel? _lyricsModel;

        public AudioModel()
        {
            Title = "Название";
            Artist = "Исполнитель";
            Image = new ImageModel
            {
                DecodeWidth = 50,
                //Bitmap = GlobalVars.DefaultMusicImage
            };
        }

        public AudioModel(Audio vkModel) : this()
        {
            if (vkModel == null)
            {
                return;
            }

            IsNotAvailable = vkModel.ContentRestricted != 0;

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

            if (vkModel.Album is {Thumb: { }})
                Image.ImageUrl = GetThumbUrl(vkModel.Album.Thumb);
        }

        /// <summary>
        /// Процент загрузки
        /// </summary>
        public int DownloadPercent
        {
            get => _downloadPercent;
            set => this.RaiseAndSetIfChanged(ref _downloadPercent, value);
        }

        /// <summary>
        /// Флаг об загрузке
        /// </summary>
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

        /// <inheritdoc/>
        public override string GetThumbUrl(AudioCover audioCover)
        {
            if (audioCover.Photo68 != null)
                return audioCover.Photo68;

            return audioCover.Photo135 ?? string.Empty;
        }

        /// <summary>
        /// Значение
        /// </summary>
        public int Duration { get; set; }


        public LyricsViewModel? LyricsViewModel
        {
            get => _lyricsModel;
            set => this.RaiseAndSetIfChanged(ref _lyricsModel, value);
        }

        public bool LyricsButtonIsVisible => false;
    }
}