using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Authorization.Module.Domain;
using Authorization.Module.Services;
using Common.Core.ToDo;
using Common.Core.Views;
using Prism.Regions;
using ReactiveUI;

namespace Authorization.Module.Views
{
    public class AuthorizationViewModel : ViewModelBase, INavigationAware
    {
        public AuthorizationViewModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            SavedAccounts = new ObservableCollection<SavedAccountModel>(_authorizationService.LoadSavedAccounts());
            SkipMenuIfOnlyOneAccount();

            SavedAccounts.CollectionChanged += (_, _) => { _authorizationService.SaveAccounts(); };

            AuthCommand = ReactiveCommand.Create(() =>
            {
                InfoText = "Открытие авторизации";
                _authCancelletionSource.ThrowIfCancellationRequested();

                Task.Run(async () =>
                {
                    try
                    {
                        _webElementServer = new WebElementServer(Port);
                        _webElementServer.ErrorEvent += WebServer_ErrorEvent;
                        _webElementServer.MessageRecived += WebServer_MessageEvent;
                        _webElementServer.StartServerOnThread();


                        while (_webElementServer.ServerStarted == false)
                        {
                            await Task.Delay(1000);
                        }

                        if (_webElementServer.ServerStarted)
                        {
                            string tmpfileEexecute = string.Empty;
                            string tmpfileEexecute2 = string.Empty;
                            string fileExecute = string.Empty;
                            string args = $"{AuthUrl} {Port}";


                            if (GlobalVars.CurrentPlatform == OSPlatform.Linux)
                            {
                                tmpfileEexecute = Path.Combine("WebElement", linuxName);
                                tmpfileEexecute2 = linuxName;
                            }

                            else if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                            {
                                tmpfileEexecute = Path.Combine("WebElement", winName);
                                tmpfileEexecute2 = winName;
                            }

                            if (File.Exists(tmpfileEexecute))
                            {
                                fileExecute = tmpfileEexecute;
                            }
                            else if (File.Exists(tmpfileEexecute2))
                            {
                                fileExecute = tmpfileEexecute2;
                            }

                            ProcessStartInfo? start = new()
                            {
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                FileName = fileExecute,
                                Arguments = args,
                            };

                            _browserProcess = new Process {StartInfo = start};


                            InfoText = "Ожидание конца авторизации";

                            _browserProcess.Start();
                            _browserProcess.WaitForExit();
                            OffServerAndUnsubscribe();
                        }
                    }
                    catch (Exception ex)
                    {
                        InfoText = "Ошибка:" + ex.Message;
                    }
                }, _authCancelletionSource);
            });

            RemoveAccountCommand = ReactiveCommand.Create<SavedAccountModel>(account =>
            {
                if (account != null)
                    SavedAccounts.Remove(account);
            });
        }

        private void OffServerAndUnsubscribe()
        {
            if (_webElementServer == null)
                return;

            _webElementServer.Stop();
            _webElementServer.ErrorEvent -= WebServer_ErrorEvent;
            _webElementServer.MessageRecived -= WebServer_MessageEvent;
        }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        /// <param name="exp"></param>
        private void WebServer_ErrorEvent(Exception exp)
        {
            InfoText = $"Произошла ошибка {exp.Message}";
            OffServerAndUnsubscribe();
        }

        private void WebServer_MessageEvent(string message)
        {
            if (!message.Contains("#access_token"))
            {
                return;
            }

            string token = message.Split("=")[1].Split("&")[0];
            string id = message.Split("=")[3].Split("&")[0];

            _browserProcess?.Kill();

            OffServerAndUnsubscribe();

            InfoText = "Авторизация успешна";
            _authorizationService.AuthByTokenAndId(token, long.Parse(id));
        }

        /// <summary>
        /// Если в списке только 1 сохраненный аккаунт, то сразу выбираем его
        /// </summary>
        private void SkipMenuIfOnlyOneAccount()
        {
            if (SavedAccounts?.Count == 1)
            {
                Authorization(SavedAccounts.First());
            }
        }

        private void OnSelectedItem()
        {
            Authorization(SelectedAccount);
        }

        private void Authorization(SavedAccountModel accountModel)
        {
            if (accountModel == null)
            {
                return;
            }

            _authorizationService.AuthorizationFromActiveAccount(accountModel);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// Сохраненные аккаунты
        /// </summary>
        public ObservableCollection<SavedAccountModel> SavedAccounts
        {
            get => _savedAccounts;
            set => this.RaiseAndSetIfChanged(ref _savedAccounts, value);
        }

        /// <summary>
        /// Сохраненные аккаунты
        /// </summary>
        public SavedAccountModel SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedAccount, value);
                OnSelectedItem();
            }
        }

        /// <summary>
        /// Информационный текст
        /// </summary>
        public string? InfoText
        {
            get => _infoText;
            set => this.RaiseAndSetIfChanged(ref _infoText, value);
        }

        private readonly IAuthorizationService _authorizationService;
        private IRegionNavigationJournal _journal;
        private WebElementServer? _webElementServer;

        public ICommand AuthCommand { get; }
        public ICommand RemoveAccountCommand { get; }


        private const string AuthUrl =
            @"https://oauth.vk.com/oauth/authorize?client_id=6463690" +
            "&scope=1073737727" +
            "&redirect_uri=https://oauth.vk.com/blank.html" +
            "&display=mobile" +
            "&response_type=token" +
            "&revoke=1";

        const int Port = 2654;
        bool _waitStartServer = false;
        string winName = "WindowsWebBrowser.exe";
        string linuxName = "Linux";

        private Process? _browserProcess;
        CancellationToken _authCancelletionSource = new();
        private ObservableCollection<SavedAccountModel> _savedAccounts;
        private SavedAccountModel _selectedAccount;
        private string? _infoText;
    }
}