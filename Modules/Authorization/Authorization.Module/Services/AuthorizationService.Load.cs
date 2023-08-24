using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Authorization.Module.Domain;
using Avalonia.Controls.Notifications;
using Common.Core.ToDo;
using JetBrains.Annotations;
using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI;

namespace Authorization.Module.Services
{
    /// <inheritdoc />
    public partial class AuthorizationService
    {
        /// <inheritdoc />
        [ItemCanBeNull]
        public ObservableCollection<SavedAccountModel>? LoadSavedAccounts()
        {
            try
            {
                if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                    LoadSavedAccountsFromRegistry();
                else LoadSavedAccountsFromConfig();

                SavedAccounts?.ToList().AsParallel().ForAll(account => account.UpdateInformation());
            }
            catch (Exception exp)
            {
                _notificationService.Show("Error", $"LoadSavedAccounts\n{exp.Message}", NotificationType.Error);
            }

            return SavedAccounts;
        }

        /// <summary>
        /// Загрузить аккаунты из Реестра
        /// </summary>
        private void LoadSavedAccountsFromRegistry()
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(GlobalVars.AppName);
                if (key == null)
                    return;

                string? data = (string?) key.GetValue(SavedAccountsFileName);
                if (data is null)
                {
                    return;
                }

                List<AccountDTO> dto = JsonConvert.DeserializeObject<List<AccountDTO>>(data);

                List<SavedAccountModel> accounts = dto.Select(accountModel => new SavedAccountModel()
                    {
                        UserId = accountModel.UserId,
                        Token = accountModel.Token,
                        IsDefault = accountModel.IsDefault,
                    })
                    .ToList();

                if (!accounts.Any())
                {
                    return;
                }

                IEnumerable<SavedAccountModel> accessAccounts = accounts.Where(a => a.Token != null);
                SavedAccounts = new ObservableCollection<SavedAccountModel>(accessAccounts);
            }
            finally
            {
                key?.Close();
            }
        }

        /// <summary>
        /// Загрузить аккаунты из файла
        /// </summary>
        private void LoadSavedAccountsFromConfig()
        {
            string? home = GlobalVars.HomeDirectory;

            if (string.IsNullOrEmpty(home))
            {
                return;
            }

            string path = Path.Combine(home, ".config", GlobalVars.AppName, SavedAccountsFileName);

            if (File.Exists(path))
            {
                SavedAccounts
                    = JsonConvert.DeserializeObject<ObservableCollection<SavedAccountModel>>(File.ReadAllText(path));
            }
        }

        /// <summary>
        /// Индекс активного аккаунты
        /// </summary>
        public int ActiveAccountSelectIndex
        {
            get => _activeAccountSelectIndex;
            private set => this.RaiseAndSetIfChanged(ref _activeAccountSelectIndex, value);
        }

        private int _activeAccountSelectIndex;
    }
}