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
using Avalonia.Controls;
using Avalonia.Input;
using Common.Core.ToDo;
using Common.Core.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Authorization.Module.Views
{
    public class AuthorizationViewModel : ViewModelBase
    {
        const int Port = 2654;
        bool _waitStartServer = false;
        string winName = "WindowsWebBrowser.exe";
        string linuxName = "Linux";

        private const string AuthUrl =
            @"https://oauth.vk.com/oauth/authorize?client_id=6463690" +
            "&scope=1073737727" +
            "&redirect_uri=https://oauth.vk.com/blank.html" +
            "&display=mobile" +
            "&response_type=token" +
            "&revoke=1";

        private Process? _browserProcess;

        CancellationToken _authCancelletionSource = new();

        private WebElementServer? _webElementServer;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationViewModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            SavedAccounts = _authorizationService.LoadSavedAccounts();

            SkipMenuIfOnlyOneAccount();
            ToggleAccountsSidebarVisible();

            SavedAccounts.CollectionChanged += (sender, args) =>
            {
                _authorizationService.SaveAccounts();
                ToggleAccountsSidebarVisible();
            };

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

        private void ToggleAccountsSidebarVisible()
        {
            SavedAccountsIsVisible = SavedAccounts?.Count > 0;
        }

        private void OffServerAndUnsubscribe()
        {
            if (_webElementServer == null)
                return;

            _webElementServer.Stop();
            _webElementServer.ErrorEvent -= WebServer_ErrorEvent;
            _webElementServer.MessageRecived -= WebServer_MessageEvent;
        }

        private void WebServer_ErrorEvent(Exception ex)
        {
            InfoText = $"Произошла ошибка {ex.Message}";
            OffServerAndUnsubscribe();
        }

        private void WebServer_MessageEvent(String message)
        {
            if (message.Contains("#access_token"))
            {
                string token = message.Split("=")[1].Split("&")[0];
                string id = message.Split("=")[3].Split("&")[0];

                _browserProcess?.Kill();

                OffServerAndUnsubscribe();
                InfoText = "Авторизация успешна";
                VkNet.VkApi? api = _authorizationService.Auth(token, long.Parse(id));
                _authorizationService.SaveAccount(api);
                // ToDo_vkApiManager.VkApi = api;
            }
        }

        private void SkipMenuIfOnlyOneAccount()
        {
            if (SavedAccounts?.Count == 1)
            {
                _authorizationService.AuthFromActiveAccount(SavedAccounts.First());
            }
        }


        public virtual void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            /*SavedAccountModel? selectedAccount = args.GetContent<SavedAccountModel>();
            if (selectedAccount != null)
                _authorizationService.AuthFromActiveAccount(sel
            ectedAccount);*/
        }

        public virtual void Scrolled(object sender, ScrollChangedEventArgs args)
        {
        }


        public ObservableCollection<SavedAccountModel>? SavedAccounts { get; set; } = new();

        [Reactive] public bool AuthButtonIsActive { get; set; }


        [Reactive] public string? InfoText { get; set; }


        [Reactive] public bool SavedAccountsIsVisible { get; set; }
        [Reactive] public int ActiveAccountSelectIndex { get; set; }


        public ICommand? AuthCommand { get; set; }
        public ICommand? RemoveAccountCommand { get; set; }
    }
}