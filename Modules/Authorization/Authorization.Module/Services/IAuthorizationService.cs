using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Authorization.Module.Domain;
using VkNet;
using VkNet.Model;

namespace Authorization.Module.Services
{
    /// <summary>
    /// Служба авторизации
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Выход пользователя из системы
        /// </summary>
        /// <returns></returns>
        void Logout();

        /// <summary>
        /// Установить регион для отображения
        /// </summary>
        /// <param name="regionName"></param>
        void SetRegionName(string regionName);

        /// <summary>
        /// Отобразить окно авторизации в регионе
        /// </summary>
        void SetAuthorizationMode();

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        Task<bool> Authorization(VkApi vkApi);

        /// <summary>
        /// Сохраненный пользователь
        /// </summary>
        public User StoredUser { get; }

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        SavedAccountModel CurrentAccount { get; set; }

        /// <summary>
        /// Загрузить сохраненных пользователей
        /// </summary>
        /// <returns></returns>
        ObservableCollection<SavedAccountModel>? LoadSavedAccounts();

        /// <summary>
        /// Сохранить аккаунт
        /// </summary>
        void SaveAccounts();

        /// <summary>
        /// Сохранить аккаунт
        /// </summary>
        void AddAccount(VkApi api);

        /// <summary>
        /// Выполнить авторизацию из активного аккаунта
        /// </summary>
        /// <param name="first"></param>
        void AuthorizationFromActiveAccount(SavedAccountModel accountModel);

        /// <summary>
        /// Выполнить авторизацию
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parse"></param>
        /// <returns></returns>
        bool AuthByTokenAndId(string token, long parse);
    }
}