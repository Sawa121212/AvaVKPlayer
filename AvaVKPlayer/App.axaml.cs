using System;
using System.Reflection;
using System.Resources;
using Authorization.Module;
using Authorization.Module.Services;
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
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using VkPlayer.Module;
using VkPlayer.Module.Views;
using VkProvider.Module;
using IResourceProvider = Common.Core.Localization.IResourceProvider;

namespace AvaVKPlayer
{
    public class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize(); // Initializes Prism.Avalonia - DO NOT REMOVE
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
            // Services 
            containerRegistry
                .RegisterSingleton<ILocalizer, Localizer>()
                .RegisterSingleton<IResourceProvider, ResourceProvider>(Assembly.GetExecutingAssembly().FullName)

                // Notification
                .RegisterSingleton<INotificationService, NotificationService>()

                // Authorization
                .RegisterSingleton<IAuthorizationService, AuthorizationService>()
                ;

            // Views - Generic
            containerRegistry.Register<ShellView>();

            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
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
                .AddModule<VkPlayerModule>()
                ;

            base.ConfigureModuleCatalog(moduleCatalog);
        }

        public override void OnFrameworkInitializationCompleted()
        {
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
            }

            Dispatcher.UIThread.InvokeAsync(() => { localizer.ChangeLanguage("ru"); },
                DispatcherPriority.SystemIdle);

            
            IRegionManager regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNameService.ShellRegionName, typeof(MainView));

            // установим регион, на котором будем показывать окно Авторизации
            IAuthorizationService? authorizationService = Container.Resolve<IAuthorizationService>();
            authorizationService.SetRegionName(RegionNameService.ShellRegionName);
            //authorizationService.SetAuthorizationMode();

            base.OnFrameworkInitializationCompleted();
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
                string viewName = viewType.FullName;
                string viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

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