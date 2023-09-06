using System;
using System.Reflection;
using System.Resources;
using Authorization.Module;
using Authorization.Module.Services;
using Authorization.Module.Views;
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
            base.Initialize(); // Initializes Prism.Avalonia
        }

        /// <summary>
        /// User interface entry point, called after Register and ConfigureModules.
        /// </summary>
        /// <returns>Startup View.</returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        /// <summary>
        /// Register Services and Views.
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
            // containerRegistry.Register<ShellView>();

            containerRegistry.RegisterForNavigation<AuthorizationView, AuthorizationViewModel>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
        }

        /// <summary>
        /// Register optional modules in the catalog.
        /// </summary>
        /// <param name="moduleCatalog">Module Catalog.</param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog
                .AddModule<VkProviderModule>()
                .AddModule<NotificationModule>()
                .AddModule<AuthorizationModule>()
                .AddModule<EqualizerModule>()
                .AddModule<VkPlayerModule>()
                ;

           // base.ConfigureModuleCatalog(moduleCatalog);
        }

        /// <summary>Called after Initialize.</summary>
        protected override void OnInitialized()
        {
            // Добавим ресурс Локализации в "коллекцию ресурсов локализации"
            ILocalizer? localizer = Container.Resolve<ILocalizer>();
            localizer.AddResourceManager(new ResourceManager(typeof(Language)));

            Dispatcher.UIThread.InvokeAsync(() => { localizer.ChangeLanguage("ru"); },
                DispatcherPriority.SystemIdle);

            // Register Views to the Region it will appear in. Don't register them in the ViewModel.
            IRegionManager regionManager = Container.Resolve<IRegionManager>();
            //Container.Resolve<MainView>();
            regionManager.RegisterViewWithRegion(RegionNameService.ShellRegionName, typeof(MainView));

            // установим регион, на котором будем показывать окно Авторизации
            IAuthorizationService? authorizationService = Container.Resolve<IAuthorizationService>();
            authorizationService.SetRegionName(RegionNameService.ShellRegionName);
            authorizationService.SetAuthorizationMode();
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