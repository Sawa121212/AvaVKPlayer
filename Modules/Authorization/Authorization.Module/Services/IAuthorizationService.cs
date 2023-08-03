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
        Task<bool> Authorization(string login, string password);

        /// <summary>
        /// Сохраненный пользователь
        /// </summary>
        public User StoredUser { get; }

        SavedAccountModel CurrentAccount { get; set; }


        ObservableCollection<SavedAccountModel>? LoadSavedAccounts();
        void SaveAccounts();
        void SaveAccount(VkApi api);
        void AuthFromActiveAccount(SavedAccountModel first);
        VkApi Auth(string token, long parse);
    }
}