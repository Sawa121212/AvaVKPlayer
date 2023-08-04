using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Authorization.Module.Domain;
using Authorization.Module.Views;
using Avalonia.Controls.Notifications;
using Common.Core.Extensions;
using Notification.Module.Services;
using Prism.Events;
using Prism.Regions;
using VkNet;
using VkNet.Model;
using VkProvider.Module;

namespace Authorization.Module.Services
{
    /// <inheritdoc />
    public partial class AuthorizationService : IAuthorizationService
    {
        private readonly INotificationService _notificationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private string _regionName;


        public AuthorizationService(
            INotificationService notificationService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _notificationService = notificationService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        /// <inheritdoc />
        public async Task<bool> Authorization(string login, string password)
        {
            if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
                _notificationService?.Show("Ошибка", "Введите все данные", NotificationType.Error);

            // Если нет авторизованного пользователя от прошлого запуска
            try
            {
                string message = "Неверный логин или пароль";
                //LoginResult result = await _scadaServerComm.Login(login, password);

                /*switch (result.State)
                {
                    case LoginState.Success:
                        await _dtoService.GetModel();
                        //_authorizedUserProvider.Set(_userRepository.Get(login));
                        _eventAggregator.GetEvent<AuthorizeEvent>().Publish(new AuthorizeEvent());

                        return true;
                    case LoginState.Failed:
                        _notificationService?.Show("Ошибка авторизации", message, NotificationType.Error);
                        return false;
                    case LoginState.PasswordChangeRequired:
                        return false;
                    default:
                        return false;
                }*/
            }
            catch (Exception e)
            {
                _notificationService?.Show("Ошибка", e.Message, NotificationType.Error);
            }

            return false;
        }


        /// <inheritdoc />
        public void Logout()
        {
            //_authorizedUserProvider.Logout();
            ShowAuthorizationView();
        }

        /// <inheritdoc />
        public void SetRegionName(string regionName)
        {
            if (!regionName.IsNullOrEmpty() && !regionName.IsWhiteSpace())
            {
                _regionName = regionName;
            }
        }

        /// <inheritdoc cref="SetAuthorizationMode" />
        public void SetAuthorizationMode()
        {
            ShowAuthorizationView();
        }

        // Показать окно авторизации
        private void ShowAuthorizationView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(_regionName))
            {
                _regionManager.RequestNavigate(_regionName, nameof(AuthorizationView));
            }
            else
            {
                _notificationService?.Show("Ошибка", $"Не найден регион: {_regionName}", NotificationType.Error);
            }
        }

        public VkApi Auth(string token, long id)
        {
            VkApi? api = new();

            api.Authorize(new ApiAuthParams {AccessToken = token, UserId = id});
            return api;
        }

        public void AuthFromActiveAccount(SavedAccountModel account)
        {
            try
            {
                CurrentAccount = account;
                VkApiManager.VkApi = Auth(account?.Token, (long) account?.UserId);
                ActiveAccountSelectIndex = -1;
            }
            catch (Exception)
            {
                ActiveAccountSelectIndex = -1;
            }
        }

        /// <inheritdoc />
        public User StoredUser { get; } // => _authorizedUserProvider.Get();

        // ToDo
        public static string SavedAccountsFileName => "Accounts";
        public SavedAccountModel? CurrentAccount { get; set; }
        public ObservableCollection<SavedAccountModel>? SavedAccounts { get; set; } = new();
    }
}