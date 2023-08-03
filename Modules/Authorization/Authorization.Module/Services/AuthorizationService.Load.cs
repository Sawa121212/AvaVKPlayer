using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Authorization.Module.Domain;
using Common.Core.ToDo;
using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using VkNet;
using VkNet.Model;

namespace Authorization.Module.Services
{
    public partial class AuthorizationService
    {
        public ObservableCollection<SavedAccountModel>? LoadSavedAccounts()
        {
            try
            {
                if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                    LoadSavedAccountsFromRegistry();

                else LoadSavedAccountsFromConfig();

                SavedAccounts?.ToList().AsParallel().ForAll(x => x.LoadAvatar());
                
            }
            catch (Exception)
            {
            }

            return SavedAccounts;
        }

        private void LoadSavedAccountsFromRegistry()
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(GlobalVars.AppName);
                if (key != null)
                {
                    string? data = (string?) key.GetValue(SavedAccountsFileName);
                    if (data is not null)
                        SavedAccounts = JsonConvert.DeserializeObject<ObservableCollection<SavedAccountModel>>(data);
                }
            }
            finally
            {
                key?.Close();
            }
        }

        private void LoadSavedAccountsFromConfig()
        {
            string? home = GlobalVars.HomeDirectory;
            if (string.IsNullOrEmpty(home)) return;
            string path = Path.Combine(home, ".config", GlobalVars.AppName, SavedAccountsFileName);
            if (File.Exists(path))
                SavedAccounts =
                    JsonConvert.DeserializeObject<ObservableCollection<SavedAccountModel>>(File.ReadAllText(path));
        }

        [Reactive] public int ActiveAccountSelectIndex { get; set; }
    }
}