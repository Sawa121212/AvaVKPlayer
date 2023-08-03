using Authorization.Module.Domain.Interfaces;
using Player.Domain.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;

namespace Player.Domain.Base
{
    public class VkAudioOrAlbumModelBase : ReactiveObject, IVkAudiOrAlbumModelBase

    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public IImageBase Image { get; set; }
        public string Subtitle { get; set; }
        public string AccessKey { get; set; }

        [Reactive] public bool IsNotAvailable { get; set; }

        public virtual string GetThumbUrl(AudioCover audioCover)
        {
            return string.Empty;
        }
    }
}