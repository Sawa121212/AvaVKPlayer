using Authorization.Module.Domain.Interfaces;
using ReactiveUI;
using VkNet.Model;
using VkPlayer.Domain.Interfaces;

namespace VkPlayer.Domain.Base
{
    public class VkAudioOrAlbumModelBase : ReactiveObject, IVkAudiOrAlbumModelBase
    {
        /// <inheritdoc/>
        public long Id { get; set; }

        /// <inheritdoc/>
        public long OwnerId { get; set; }

        /// <inheritdoc/>
        public string Artist { get; set; }

        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public IImageBase Image { get; set; }

        /// <inheritdoc/>
        public string Subtitle { get; set; }

        /// <inheritdoc/>
        public string AccessKey { get; set; }

        /// <summary>
        /// Флаг, указывающий доступность
        /// </summary>
        
        public bool IsNotAvailable { get; set; }

        /// <summary>
        /// Получить обложка
        /// </summary>
        /// <param name="audioCover">Обложка аудиоальбома</param>
        /// <returns></returns>
        public virtual string GetThumbUrl(AudioCover audioCover)
        {
            return string.Empty;
        }
    }
}