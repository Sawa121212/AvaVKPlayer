using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Authorization.Module.Domain;
using Avalonia.Controls.Notifications;
using Common.Core.Extensions;
using Common.Core.ToDo;
using Microsoft.Win32;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model;

namespace Authorization.Module.Services
{
    public partial class AuthorizationService
    {
        /// <inheritdoc />
        public void AddAccount(VkApi? vkApi)
        {
            IEnumerable<SavedAccountModel>? accountEnumerable =
                SavedAccounts?.ToList().Where(x => x.UserId == vkApi?.UserId);

            if (accountEnumerable != null)
            {
                foreach (SavedAccountModel? savedAccountModel in accountEnumerable)
                {
                    SavedAccounts?.Remove(savedAccountModel);
                }
            }

            AccountSaveProfileInfoParams accountData = null;
            try
            {
                accountData = vkApi?.Account?.GetProfileInfo();
            }
            catch (Exception exp)
            {
                _notificationService.Show("Error", $"SaveAccount\n{exp.Message}", NotificationType.Error);
            }

            SavedAccounts?.Insert(0, new SavedAccountModel()
            {
                Token = vkApi.Token,
                UserId = vkApi.UserId,
                Name = $"{accountData.FirstName} {accountData.LastName}",
                Status = accountData.Status
            });

            CurrentAccount = SavedAccounts.First();

            SaveAccounts();
        }

        /// <inheritdoc />
        public void SaveAccounts()
        {
            List<AccountDTO> dto = SavedAccounts.Select(accountModel => new AccountDTO()
                {
                    UserId = accountModel.UserId,
                    Token = accountModel.Token,
                    IsDefault = accountModel.IsDefault,
                })
                .ToList();

            if (!dto.Any())
            {
                return;
            }

            string saveText = JsonConvert.SerializeObject(dto);

            if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                SaveAccountsOnRegistry(saveText);
            else SaveAccountsOnConfig(saveText);
        }

        private void SaveAccountsOnRegistry(string? data)
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(GlobalVars.AppName);
                key?.SetValue(SavedAccountsFileName, data ?? string.Empty);
            }
            finally
            {
                key?.Close();
            }
        }

        private void SaveAccountsOnConfig(string? data)
        {
            string? home = GlobalVars.HomeDirectory;
            if (string.IsNullOrEmpty(home)) return;
            string path = Path.Combine(home, ".config", GlobalVars.AppName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, SavedAccountsFileName);
            File.WriteAllText(path, data);
        }
    }
}