using Authorization.Module.Events;
using Common.Core.Regions;
using Prism.Commands;
using Prism.Regions;
using VkPlayer.Module.Views.Pages;

namespace VkPlayer.Module.Views
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

        /// <summary>
        /// Показать главное отображение
        /// </summary>
        private void OnShowMainView(AuthorizeEvent authorizeEvent)
        {
            _regionManager.RequestNavigate(RegionNameService.ShellRegionName, nameof(MainView));
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