using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Authorization.Module.Domain;
using Common.Core.ToDo;
using Microsoft.Win32;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model;

namespace Authorization.Module.Services
{
    public partial class AuthorizationService
    {
        public void SaveAccount(VkApi? vkApi)
        {
            IEnumerable<SavedAccountModel>? accountEnumerable =
                SavedAccounts?.ToList().Where(x => x.UserId == vkApi?.UserId);

            if (accountEnumerable != null)
                foreach (SavedAccountModel? savedAccountModel in accountEnumerable)
                    SavedAccounts?.Remove(savedAccountModel);

            AccountSaveProfileInfoParams accountData = null;
            try
            {
                accountData = vkApi?.Account?.GetProfileInfo();
            }
            catch (Exception ex)
            {
                //ToDo InfoText = "Ошибка:" + ex.Message;
            }

            SavedAccounts?.Insert(0,
                new SavedAccountModel
                {
                    Token = vkApi.Token,
                    UserId = vkApi.UserId,
                    Name = $"{accountData.FirstName} {accountData.LastName}"
                });

            CurrentAccount = SavedAccounts.First();
        }

        public void SaveAccounts()
        {
            string saveText = JsonConvert.SerializeObject(SavedAccounts);
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