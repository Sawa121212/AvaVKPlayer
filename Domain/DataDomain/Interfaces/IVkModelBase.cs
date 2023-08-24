using Authorization.Module.Domain.Interfaces;

namespace VkPlayer.Domain.Interfaces
{
    /// <summary>
    /// Вложение
    /// </summary>
    public interface IVkModelBase
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Обложка
        /// </summary>
        public IImageBase Image { get; }
    }
}