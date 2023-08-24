namespace VkPlayer.Domain.Interfaces
{
    /// <summary>
    /// Медиа: музыка или альбом
    /// </summary>
    public interface IVkAudiOrAlbumModelBase : IVkModelBase
    {
        /// <summary>
        /// Идентификатор владельца вложения
        /// </summary>
        public long OwnerId { get; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public string Artist { get; }

        /// <summary>
        /// Ключ доступа
        /// </summary>
        public string AccessKey { get; }

        /// <summary>
        /// Подзаголовок вложения
        /// </summary>
        public string Subtitle { get; }
    }
}