using System.Diagnostics;
using System.Windows.Input;
using Common.Core.Views;
using Prism.Commands;
using Prism.Regions;

namespace Player.Module.Views.Pages
{
    /// <summary>
    /// О программе
    /// </summary>
    public class AboutViewModel : ViewModelBase, INavigationAware
    {
        private IRegionNavigationJournal _journal;

        public AboutViewModel()
        {
            MoveBackCommand = new DelegateCommand(OnGoBack);
            OpenGitHubLinkCommand = new DelegateCommand(OnOpenProjectRepoLink);
            OpenAvaloniaCommand = new DelegateCommand(OnOpenAvaloniaLink);
            OpenPrismAvaloniaCommand = new DelegateCommand(OnOpenPrismAvaloniaLink);
            OpenMaterialAvaloniaCommand = new DelegateCommand(OnOpenMaterialAvaloniaLink);
        }

        private void OnOpenProjectRepoLink() =>
            OpenBrowserForVisitSite("https://github.com/Sawa121212/AvaVKPlayer");

        private void OnOpenAvaloniaLink() =>
            OpenBrowserForVisitSite("https://avaloniaui.net/");

        private void OnOpenMaterialAvaloniaLink() =>
            OpenBrowserForVisitSite("https://github.com/AvaloniaUtils/material.avalonia");

        private void OnOpenPrismAvaloniaLink() =>
            OpenBrowserForVisitSite("https://github.com/AvaloniaCommunity/Prism.Avalonia");

        private void OpenBrowserForVisitSite(string link)
        {
            ProcessStartInfo param = new()
            {
                FileName = link,
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(param);
        }

        private void OnGoBack()
        {
            _journal.GoBack();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _journal = navigationContext.NavigationService.Journal;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public ICommand MoveBackCommand { get; }
        public ICommand OpenGitHubLinkCommand { get; }
        public ICommand OpenAvaloniaCommand { get; }
        public ICommand OpenPrismAvaloniaCommand { get; }
        public ICommand OpenMaterialAvaloniaCommand { get; }
    }
}