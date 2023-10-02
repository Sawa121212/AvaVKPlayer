using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Module.Domain;
using Authorization.Module.Events;
using Authorization.Module.Views;
using Avalonia.Controls.Notifications;
using Common.Core.Extensions;
using Notification.Module.Services;
using Prism.Events;
using Prism.Regions;
using ReactiveUI;
using VkNet;
using VkNet.Model;
using VkProvider.Module;

namespace Authorization.Module.Services
{
    /// <inheritdoc />
    public partial class AuthorizationService : ReactiveObject, IAuthorizationService
    {
        public AuthorizationService(
            INotificationService notificationService,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _notificationService = notificationService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            SavedAccounts = new ObservableCollection<SavedAccountModel>();
        }

        /// <inheritdoc />
        public async Task<bool> Authorization(VkApi vkApi)
        {
            // Если нет авторизованного пользователя от прошлого запуска
            try
            {
                if (SavedAccounts != null && SavedAccounts.Any())
                {
                    //var account = SavedAccounts.
                }


                //string message = "Неверный логин или пароль";
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
                return false;
            }
            catch (Exception e)
            {
                _notificationService?.Show("Ошибка", e.Message, NotificationType.Error);
            }

            return false;
        }


        /// <inheritdoc />
        public void LogOut()
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

        /// <summary>
        /// Показать окно авторизации
        /// </summary>
        private void ShowAuthorizationView()
        {
            if (_regionManager.Regions.ContainsRegionWithName(_regionName))
            {
                _regionManager.RequestNavigate(_regionName, nameof(AuthorizationView));
            }
            else
            {
                _regionManager.RegisterViewWithRegion(_regionName, nameof(AuthorizationView));
            }
            /*else
            {
                _notificationService?.Show("Ошибка", $"Не найден регион: {_regionName}", NotificationType.Error);
            }*/
        }

        /// <inheritdoc />
        public bool AuthByTokenAndId(string token, long id)
        {
            VkApi api = Auth(token, id);
            if (api == null)
            {
                return false;
            }

            AddAccount(api);
            VkApiManager.VkApi = api;

            return true;
        }

        private VkApi Auth(string token, long id)
        {
            if (token == null)
            {
                return null;
            }

            VkApi? api = new();

            api.Authorize(new ApiAuthParams {AccessToken = token, UserId = id});

            return !api.IsAuthorized ? null : api;
        }

        /// <inheritdoc />
        public void AuthorizationFromActiveAccount(SavedAccountModel account)
        {
            try
            {
                CurrentAccount = account;
                VkApiManager.VkApi = Auth(account?.Token, (long) account?.UserId);
                ActiveAccountSelectIndex = -1;
                _eventAggregator.GetEvent<AuthorizeEvent>().Publish(new AuthorizeEvent());
            }
            catch (Exception exp)
            {
                ActiveAccountSelectIndex = -1;
                _notificationService.Show("Error", $"AuthFromActiveAccount\n{exp.Message}", NotificationType.Error);
            }
        }

        /// <inheritdoc />
        public User StoredUser { get; } // => _authorizedUserProvider.Get();


        /// <inheritdoc />
        public SavedAccountModel? CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        /// <summary>
        /// Коллекция сохраненных аккаунтов
        /// </summary>
        private ObservableCollection<SavedAccountModel> SavedAccounts
        {
            get => _savedAccounts;
            set => this.RaiseAndSetIfChanged(ref _savedAccounts, value);
        }

        // ToDo
        public static string SavedAccountsFileName => "Accounts";

        private readonly INotificationService _notificationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private string _regionName;
        private SavedAccountModel _currentAccount;
        private ObservableCollection<SavedAccountModel> _savedAccounts;
    }
}