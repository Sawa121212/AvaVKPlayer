using Common.Core.Regions;
using Common.Resources.m3.Navigation;
using Player.Module.Views.Pages;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Player.Module.Views
{
    public partial class MainViewModel
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;

        /// <summary>
        /// Показать настройки приложения
        /// </summary>
        private void OnShowSettings()
        {
            _regionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(SettingsView));
        }

        /// <summary>
        /// Показать информацию о приложении
        /// </summary>
        private void OnShowAbout()
        {
            _regionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(AboutView));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _journal = navigationContext.NavigationService.Journal;
            ShowSettingsCommand.RaiseCanExecuteChanged();
            ShowAboutCommand.RaiseCanExecuteChanged();
        }


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public DelegateCommand ShowSettingsCommand { get; }
        public DelegateCommand ShowAboutCommand { get; }
    }
}