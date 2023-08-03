using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Common.Core.Extensions;
using Common.Core.Localization;
using Common.Core.Views;

namespace AvaVKPlayer.Views
{
    public class ShellViewModel : ViewModelBase
    {
        private LanguagesEnum _appCultureInfo;
        private readonly ILocalizer _localizer;

        public ShellViewModel(ILocalizer localizer)
        {
            _localizer = localizer;

            // Change the resource language forcibly during initialization
            // Изменим язык ресурсов принудительно при инициализации
            OnChangeLanguage();
        }

        /// <summary>
        /// Поменять язык
        /// </summary>
        private async Task OnChangeLanguage()
        {
            _appCultureInfo = CultureInfo.CurrentUICulture.ToString().ToEnum<LanguagesEnum>();

            if (_appCultureInfo != null)
            {
                // lang
                await Dispatcher.UIThread.InvokeAsync(() => { OnChangeCulture(_appCultureInfo); },
                    DispatcherPriority.SystemIdle);
            }
        }

        private void OnChangeCulture(LanguagesEnum languagesEnum)
        {
            string lang = languagesEnum.ToString();
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            _localizer.EditLn(lang);
        }

        public string Title => "AvaVKPlayer";
    }
}