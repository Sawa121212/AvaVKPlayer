using System;
using System.Reflection;
using System.Resources;
using Authorization.Module;
using Authorization.Module.Services;
using Authorization.Module.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaVKPlayer.Properties;
using AvaVKPlayer.Views;
using Common.Core.Localization;
using Common.Core.Regions;
using Equalizer.Module;
using Notification.Module;
using Notification.Module.Services;
using Player.Module;
using Player.Module.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using VkProvider.Module;
using IResourceProvider = Common.Core.Localization.IResourceProvider;

namespace AvaVKPlayer
{
    public class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            // Initializes Prism.Avalonia - DO NOT REMOVE
            base.Initialize();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        /// <summary>
        /// Регистрация служб и отображений приложения
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Регистрация General служб приложения 
            containerRegistry
                .RegisterSingleton<ILocalizer, Localizer>()
                .RegisterSingleton<IResourceProvider, ResourceProvider>(Assembly.GetExecutingAssembly().FullName)

                // Notification
                .RegisterSingleton<INotificationService, NotificationService>()
                .RegisterSingleton<IAuthorizationService, AuthorizationService>()
                ;

            // Views - Generic
            containerRegistry.RegisterSingleton<ShellView>();
        }

        /// <summary>
        /// Регистрация модулей приложения
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog
                .AddModule<VkProviderModule>()
                .AddModule<NotificationModule>()
                .AddModule<AuthorizationModule>()
                .AddModule<EqualizerModule>()
                .AddModule<PlayerModule>()
                ;

            base.ConfigureModuleCatalog(moduleCatalog);
        }

        protected override void InitializeShell(IAvaloniaObject shell)
        {
            base.InitializeShell(shell);

            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return;

            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            ILocalizer? localizer = Container.Resolve<ILocalizer>();
            localizer.AddResourceManager(new ResourceManager(typeof(Language)));

            desktop.MainWindow = CreateShell();
            if (desktop.MainWindow != null)
            {
                INotificationService? notifyService = Container.Resolve<INotificationService>();
                notifyService.SetHostWindow(desktop.MainWindow);
                desktop.MainWindow.Show();
            }

            Dispatcher.UIThread.InvokeAsync(() => { localizer.ChangeLanguage("ru"); },
                DispatcherPriority.SystemIdle);
        }

        /// <summary>Called after <seealso cref="Initialize"/>.</summary>
        protected override void OnInitialized()
        {
            // ToDo: remove line
            Container.Resolve<IRegionManager>().RequestNavigate(RegionNameService.ShellRegionName, nameof(MainView));

            // ToDo: uncomment lines
            // установим регион, на котором будем показывать окно Авторизации
            /*IAuthorizationService? authorizationService = Container.Resolve<IAuthorizationService>();
            authorizationService.SetRegionName(RegionNameService.ShellRegionName);
            authorizationService.SetAuthorizationMode();*/
        }

        /// <summary>
        /// ViewModel Locator. Мы работаем с View, а не с ViewModel!
        /// Ищем ViewModel для View в той же папке, где и View лежит.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                string? viewName = viewType.FullName;
                string? viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                string viewModelName = string.Format(
                    viewName != null && viewName.EndsWith("View", StringComparison.OrdinalIgnoreCase)
                        ? "{0}Model, {1}"
                        : "{0}ViewModel, {1}",
                    viewName, viewAssemblyName);
                return Type.GetType(viewModelName);
            });

            ViewModelLocationProvider.SetDefaultViewModelFactory(type => Container.Resolve(type));
        }
    }
}