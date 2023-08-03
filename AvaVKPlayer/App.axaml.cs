using System;
using System.Reflection;
using System.Resources;
using Authorization.Module;
using Authorization.Module.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaVKPlayer.Properties;
using AvaVKPlayer.Views;
using Common.Core.Localization;
using Common.Core.Regions;
using DryIoc;
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
            base.Initialize();
        }

        private Window GetAppShell()
        {
            return Container.Resolve<ShellView>();
        }

        /// <summary>
        /// Регистрация служб приложения
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
            containerRegistry.Register<ShellView>();

            containerRegistry.RegisterForNavigation<MainView>();
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

        protected override Window CreateShell()
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            {
                throw new Exception(@"ApplicationLifetime is not {nameof(IClassicDesktopStyleApplicationLifetime)}");
            }

            // Добавим локализацию
            ILocalizer? localizer = Container.Resolve<ILocalizer>();
            localizer.AddResourceManager(new ResourceManager(typeof(Language)));

            Window? mainWindow = GetAppShell();

            if (mainWindow != null)
            {
                desktop.MainWindow = mainWindow;

                INotificationService? notifyService = Container.Resolve<INotificationService>();
                notifyService.SetHostWindow(desktop.MainWindow);
            }

            Dispatcher.UIThread.InvokeAsync(() => { localizer.ChangeLanguage("ru"); },
                DispatcherPriority.SystemIdle);

            return desktop.MainWindow;
        }

        /// <summary>Called after <seealso cref="Initialize"/>.</summary>
        protected override void OnInitialized()
        {
            // установим регин, на котором будем показывать окно Авторизации
            Container.Resolve<IAuthorizationService>().SetRegionName(RegionNameService.ShellRegionName);

            // Register initial Views to Region.
            IRegionManager? regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNameService.ShellRegionName, nameof(MainView));
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