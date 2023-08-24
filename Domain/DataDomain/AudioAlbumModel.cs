using Authorization.Module.Domain;
using VkNet.Model;
using VkPlayer.Domain.Base;

namespace VkPlayer.Domain
{
    /// <summary>
    /// Альбом
    /// </summary>
    public class AudioAlbumModel : VkAudioOrAlbumModelBase
    {
        public AudioAlbumModel(AudioPlaylist audioPlaylist)
        {
            Image = new ImageModel
            {
                //Bitmap = GlobalVars.DefaultAlbumImage,
                DecodeWidth = 0
            };

            Title = audioPlaylist.Title;
            Id = (long) audioPlaylist.Id;
            OwnerId = (long) audioPlaylist.OwnerId;
            IsFollowing = audioPlaylist.IsFollowing;

            if (audioPlaylist.Photo != null)
                Image.ImageUrl = GetThumbUrl(audioPlaylist.Photo);
        }

        /// <summary>
        /// Флаг о подписке на альбом 
        /// </summary>
        public bool IsFollowing { get; }

        /// <inheritdoc />
        public override string GetThumbUrl(AudioCover audioCover)
        {
            if (audioCover.Photo135 != null)
                return audioCover.Photo135;

            return audioCover.Photo270 ?? string.Empty;
        }
    }
}