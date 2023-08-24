namespace Authorization.Module.Domain
{
    /// <summary>
    /// Сохраненный аккаунт
    /// </summary>
    public class AccountDTO
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Токен
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Флаг, указывающий что этот аккаунт выбран для входа по умолчанию
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}