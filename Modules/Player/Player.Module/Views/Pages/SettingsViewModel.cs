using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Common.Core.Extensions;
using Common.Core.Localization;
using Common.Core.Views;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Prism.Commands;
using Prism.Regions;
using ReactiveUI;

namespace VkPlayer.Module.Views.Pages
{
    public class SettingsViewModel : ViewModelBase, INavigationAware
    {
        public SettingsViewModel(ILocalizer localizer)
        {
            _localizer = localizer;
            ChangeLanguageCommand = new DelegateCommand(async () => await OnChangeLanguage());
            ChangeMaterialUiThemeCommand = new DelegateCommand(OnChangeMaterialUiTheme);
            MoveBackCommand = new DelegateCommand(OnGoBack);
            CultureInfo = System.Globalization.CultureInfo.CurrentUICulture.ToString().ToEnum<LanguagesEnum>();
            _currentAppCultureInfo = CultureInfo;
            GetTheme();
            _initialized = true;
        }

        private void GetTheme()
        {
            _paletteHelper = new PaletteHelper();
            ITheme theme = _paletteHelper.GetTheme();
            ThemeMode = theme.Background.Equals(Colors.Black) ? BaseThemeMode.Dark : BaseThemeMode.Light;
            _currentThemeMode = ThemeMode;
        }

        /// <summary>
        /// Поменять язык
        /// </summary>
        private async Task OnChangeLanguage()
        {
            if (!_currentAppCultureInfo.Equals(CultureInfo) || !_initialized)
            {
                // lang
                await Dispatcher.UIThread.InvokeAsync(
                    () => { OnChangeCulture(CultureInfo); },
                    DispatcherPriority.SystemIdle);
                _currentAppCultureInfo = CultureInfo;
            }
        }

        private void OnChangeCulture(LanguagesEnum languagesEnum)
        {
            string lang = languagesEnum.ToString();
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(lang);
            _localizer.EditLn(lang);
        }

        /// <summary>
        /// Поменять тему
        /// </summary>
        private void OnChangeMaterialUiTheme()
        {
            if (!_currentThemeMode.Equals(ThemeMode))
            {
                //Retrieve the app's existing theme
                ITheme theme = _paletteHelper.GetTheme();

                //Change the base theme
                switch (ThemeMode)
                {
                    case BaseThemeMode.Inherit:
                    case BaseThemeMode.Light:
                        theme.SetBaseTheme(BaseThemeMode.Light.GetBaseTheme());
                        break;
                    case BaseThemeMode.Dark:
                        theme.SetBaseTheme(BaseThemeMode.Dark.GetBaseTheme());
                        break;
                    default:
                        break;
                }

                //Change the app's current theme
                _paletteHelper.SetTheme(theme);
                _currentThemeMode = ThemeMode;
            }
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

        /// <summary>
        /// Application Language / Язык приложения
        /// </summary>

        public LanguagesEnum CultureInfo
        {
            get => _cultureInfo;
            set => this.RaiseAndSetIfChanged(ref _cultureInfo, value);
        }

        /// <summary>
        /// Тема приложения
        /// </summary>

        public BaseThemeMode ThemeMode
        {
            get => _themeMode;
            set => this.RaiseAndSetIfChanged(ref _themeMode, value);
        }

        public ICommand MoveBackCommand { get; }
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ChangeMaterialUiThemeCommand { get; }


        private readonly ILocalizer _localizer;
        private readonly IRegionManager _regionManager;
        private LanguagesEnum _currentAppCultureInfo;
        private BaseThemeMode _currentThemeMode;
        private PaletteHelper _paletteHelper;
        private IRegionNavigationJournal _journal;
        private bool _initialized;
        private BaseThemeMode _themeMode;
        private LanguagesEnum _cultureInfo;
    }
}