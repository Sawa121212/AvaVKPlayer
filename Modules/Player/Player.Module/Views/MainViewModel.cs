using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Authorization.Module.Domain;
using Authorization.Module.Events;
using Authorization.Module.Services;
using Authorization.Module.Views;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Threading;
using Common.Core.ToDo;
using Common.Core.Views;
using Common.Resources.m3.Navigation;
using Material.Icons;
using Material.Icons.Avalonia;
using Notification.Module.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using ReactiveUI;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkPlayer.Module.ViewModels.Audios;
using VkPlayer.Module.ViewModels.Audios.Albums;
using VkPlayer.Module.ViewModels.Base;
using VkPlayer.Module.ViewModels.Exceptions;

namespace VkPlayer.Module.Views
{
    public partial class MainViewModel : ViewModelBase, INavigationAware
    {
        public MainViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IAuthorizationService authorizationService,
            IRegionManager regionManager,
            INotificationService notificationService)
        {
            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _regionManager = regionManager;

            OnUpdateCurrentAccountInfo();

            ShowSettingsCommand = new DelegateCommand(OnShowSettings);
            ShowAboutCommand = new DelegateCommand(OnShowAbout);
            LogOutCommand = new DelegateCommand(OnLogOut);

            MenuColumnWidth = new GridLength(50);
            IsMaximized = true;

            PlayerContext = PlayerControlViewModel.Instance;

            PlayerContext.AudioChangedEvent += PlayerContext_AudioChangedEvent;
            //ToDo Events.AudioRepostEvent += Events_AudioRepostEvent;
            //ToDo Events.AudioAddToAlbumEvent += Events_AudioAddToAlbumEvent;
            ExceptionViewModel.ViewExitEvent += ExceptionViewModel_ViewExitEvent;

            VkLoginViewModel = new AuthorizationViewModel(_authorizationService);

            // ToDo in ShellView
            OpenHideMiniPlayerCommand = ReactiveCommand.Create(() =>
            {
                /*
                                if (IsMaximized)
                                {
                                    IsMaximized = false;
                                    _oldheight = MainView.Instance.Height;
                                    MenuColumnWidth = new GridLength(0);
                                    MainView.Instance.Topmost = true;
                                    MainView.Instance.MinHeight = 100;
                                    MainView.Instance.Height = 100;
                                    MainView.Instance.MaxHeight = 100;
                                }
                                else
                                {
                                    IsMaximized = true;
                                    if (_menuIsOpen)
                                    {
                                        MenuColumnWidth = new GridLength(200);
                                    }
                                    else MenuColumnWidth = new GridLength(60);
                
                                    MainView.Instance.Topmost = false;
                                    MainView.Instance.MaxHeight = Double.PositiveInfinity;
                                    MainView.Instance.MinHeight = 400;
                                    MainView.Instance.Height = _oldheight;
                                }*/
            });

            InvokeHandler.TaskErrorResponsedEvent += (handlerObject, exception) =>
            {
                //ToDo
                /*if (exception is UserAuthorizationFailException)
                {
                    ExceptionIsVisible = true;
                    handlerObject.View.ExceptionModel = new ExceptionViewModel
                    {
                        Action = AccountExit,
                        View = handlerObject.View,
                        ErrorMessage = "Ошибка: требуется авторизация",
                        ButtonMessage = "Открыть авторизацию",
                    };
                }
                else
                {
                    handlerObject.View.IsError = true;
                    handlerObject.View.ExceptionModel = new ExceptionViewModel
                    {
                        Action = handlerObject.Action,
                        View = handlerObject.View,
                        ErrorMessage = "Ошибка:" + exception.Message,
                        ButtonMessage = "Повторить",
                    };
                }

                if (CurrentAudioViewModel == null)
                {
                    ExceptionViewModel = handlerObject.View.ExceptionModel;
                    ExceptionIsVisible = true;
                }
                else if (CurrentAudioViewModel.GetType().Name == handlerObject.Action.Target.GetType().Name)
                {
                    ExceptionViewModel = CurrentAudioViewModel.ExceptionModel;
                    ExceptionIsVisible = true;
                }*/
            };

            // Раскрыть меню
            AvatarPressedCommand = ReactiveCommand.Create(() =>
            {
                switch (_siderBarAnimationIsPlaying)
                {
                    case false when !_menuIsOpen:
                        _siderBarAnimationIsPlaying = true;
                        Task.Run(async () =>
                        {
                            /*for (int i = 60; i < 200; i += 5)
                            {
                                MenuColumnWidth = new GridLength(i);
                                await Task.Delay(new TimeSpan(0, 0, 0, 0, 1));
                            }*/
                            MenuColumnWidth = new GridLength(200);
                            MenuTextIsVisible = true;
                            _siderBarAnimationIsPlaying = false;
                            _menuIsOpen = true;
                        });
                        break;

                    case false when _menuIsOpen:
                        _siderBarAnimationIsPlaying = true;
                        Task.Run(async () =>
                        {
                            /*for (int i = 200; i >= 60; i -= 5)
                            {
                                MenuColumnWidth = new GridLength(i);
                                await Task.Delay(new TimeSpan(0, 0, 0, 0, 1));
                            }*/
                            MenuColumnWidth = new GridLength(60);
                            MenuTextIsVisible = false;
                            _siderBarAnimationIsPlaying = false;
                            _menuIsOpen = false;
                        });
                        break;
                }
            });

            _searchViewModel = new AudioSearchViewModel();

            _eventAggregator.GetEvent<AuthorizeEvent>().Subscribe(OnShowMainView);

            this.WhenAnyValue(vm => vm.MenuSelectionIndex).Subscribe(OpenViewFromMenu);

            NavigationMenu = new NavigationMenu();

            NavigationMenu.AddItem(new NavigationItem()
            {
                Index = 0,
                Title = "Текущий плейлист",
                Icon = new MaterialIcon() {Kind = MaterialIconKind.MusicBoxMultiple, Width = 32, Height = 32},
                ToolTip = "Плейлист",
                Content = _containerProvider.Resolve<MusicListControlView>(),
                DataContext = _currentMusicListViewModel
            });

            NavigationMenu.AddItem(new NavigationItem()
            {
                Index = 1,
                Title = "Музыка",
                Icon = new MaterialIcon() {Kind = MaterialIconKind.Music, Width = 32, Height = 32},
                ToolTip = "Музыка",
                Content = _containerProvider.Resolve<MusicListControlView>(),
                DataContext = _allMusicListViewModel
            });

            NavigationMenu.AddItem(new NavigationItem()
            {
                Index = 2,
                Title = "Альбомы",
                Icon = new MaterialIcon() {Kind = MaterialIconKind.Album, Width = 32, Height = 32},
                ToolTip = "Альбомы",
                Content = _containerProvider.Resolve<AlbumListControl>(),
                DataContext = typeof(OpenAlbumViewModel)
            });

            NavigationMenu.AddItem(new NavigationItem()
            {
                Index = 3,
                Title = "Поиск",
                Icon = new MaterialIcon() {Kind = MaterialIconKind.Search, Width = 32, Height = 32},
                ToolTip = "Поиск",
                Content = _containerProvider.Resolve<MusicListControlView>(),
                DataContext = _searchViewModel
            });

            NavigationMenu.AddItem(new NavigationItem()
            {
                Index = 4,
                Title = "Рекомендации",
                Icon = new MaterialIcon() {Kind = MaterialIconKind.ThumbUp, Width = 32, Height = 32},
                ToolTip = "Рекомендации",
                Content = _containerProvider.Resolve<MusicListControlView>(),
                DataContext = _currentMusicListViewModel
            });
        }


        private void Events_AudioAddToAlbumEvent(AudioModel audiomodel)
        {
            AddToAlbumViewModel = new AddToAlbumViewModel(audiomodel, _authorizationService, _notificationService);
            AddToAlbumViewModel.StartLoad();
            AddToAlbumViewModel.CloseViewEvent += AddToAlbumViewModel_CloseViewEvent;
        }

        private void AddToAlbumViewModel_CloseViewEvent()
        {
            AddToAlbumViewModel.CloseViewEvent -= AddToAlbumViewModel_CloseViewEvent;
            AddToAlbumViewModel?.DataCollection?.Clear();
            AddToAlbumViewModel = null;
        }

        private void PlayerContext_AudioChangedEvent(AudioModel? model)
        {
            CurrentAudioViewModel.SelectToModel(model, true);
        }

        private void Events_AudioRepostEvent(AudioModel audioModel)
        {
            RepostViewModel = new RepostViewModel(RepostToType.Friend, audioModel);
            RepostViewModel.CloseViewEvent += RepostViewModel_CloseViewEvent;
        }

        private void RepostViewModel_CloseViewEvent()
        {
            RepostViewModel.CloseViewEvent -= RepostViewModel_CloseViewEvent;
            RepostViewModel?.DataCollection?.Clear();
            RepostViewModel = null;
        }

        public void OpenViewFromMenu(int menuIndex)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ExceptionIsVisible = false;
                CurrentAudioViewModel = null;
                ContentControl content = NavigationMenuSelection.Content;

                switch (menuIndex)
                {
                    case 0:
                    {
                        CurrentAudioViewModel = _currentMusicListViewModel;
                        CurrentAudioViewModel?.SelectToModel(PlayerContext?.CurrentAudio, true);
                        content.DataContext = CurrentAudioViewModel;
                        break;
                    }
                    case 1:
                    {
                        if (_allMusicListViewModel == null)
                        {
                            _allMusicListViewModel = new AllMusicViewModel(_notificationService);
                            _allMusicListViewModel.StartLoad();
                        }

                        CurrentAudioViewModel = _allMusicListViewModel;
                        CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, true);
                        content.DataContext = CurrentAudioViewModel;
                        break;
                    }
                    case 2:
                    {
                        if (AlbumsViewModel == null)
                        {
                            AlbumsViewModel = new OpenAlbumViewModel(_authorizationService);
                            AlbumsViewModel.StartLoad();
                        }

                        content.DataContext = AlbumsViewModel;
                        break;
                    }
                    case 3:
                    {
                        CurrentAudioViewModel = _searchViewModel;
                        if (CurrentAudioViewModel != null)
                        {
                            CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, true);
                            content.DataContext = CurrentAudioViewModel;
                        }

                        break;
                    }
                    case 4:
                    {
                        if (_recomendationsViewModel is null)
                        {
                            _recomendationsViewModel = new RecomendationsViewModel();
                            _recomendationsViewModel.StartLoad();
                        }

                        CurrentAudioViewModel = _recomendationsViewModel;
                        CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, true);
                        content.DataContext = CurrentAudioViewModel;
                        break;
                    }
                    case 5:
                    {
                        OnLogOut();
                        break;
                    }
                }
            });
        }

        /// <summary>
        /// Обновить информацию о текущем аккаунте
        /// </summary>
        private void OnUpdateCurrentAccountInfo()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    _currentMusicListViewModel = new CurrentMusicListViewModel();
                    CurrentAccountModel = _authorizationService.CurrentAccount;
                    MenuSelectionIndex = 1;

                    /*if (CurrentAccountModel.Image is null)
                        CurrentAccountModel.LoadAvatar();*/
                }
                catch (Exception exp)
                {
                    _notificationService.Show("Error",
                        $"Ошибка при обновлении информации о текущем аккаунте.\n{exp.Message}", NotificationType.Error);
                }
            });
        }

        /// <summary>
        /// Выйти из аккаунта
        /// </summary>
        private void OnLogOut()
        {
            try
            {
                PlayerContext.CurrentAudio = null;
            }
            catch (Exception exp)
            {
                _notificationService.Show("Error",
                    $"Ошибка при очистке информации о музыке.\n{exp.Message}", NotificationType.Error);
            }

            ExceptionIsVisible = false;

            AlbumsViewModel?.DataCollection?.Clear();
            _recomendationsViewModel?.DataCollection?.Clear();
            _allMusicListViewModel?.DataCollection?.Clear();
            _searchViewModel?.DataCollection?.Clear();
            RepostViewModel?.DataCollection?.Clear();
            AddToAlbumViewModel?.DataCollection?.Clear();

            RepostViewModel = null;
            AddToAlbumViewModel = null;
            CurrentAudioViewModel = null;
            AlbumsViewModel = null;
            _recomendationsViewModel = null;
            _allMusicListViewModel = null;
            _searchViewModel = null;
            CurrentAccountModel = null;

            GC.Collect(0, GCCollectionMode.Optimized);
            GC.Collect(1, GCCollectionMode.Optimized);
            GC.Collect(2, GCCollectionMode.Optimized);
            GC.Collect(3, GCCollectionMode.Optimized);

            OnUpdateCurrentAccountInfo();

            _authorizationService.LogOut();
        }

        private void ExceptionViewModel_ViewExitEvent()
        {
            CurrentAudioViewModel.IsLoading = true;
            CurrentAudioViewModel.IsError = false;
            ExceptionIsVisible = false;
        }


        public PlayerControlViewModel PlayerContext
        {
            get => _playerContext;
            set => this.RaiseAndSetIfChanged(ref _playerContext, value);
        }

        public AuthorizationViewModel? VkLoginViewModel
        {
            get => _vkLoginViewModel;
            set => this.RaiseAndSetIfChanged(ref _vkLoginViewModel, value);
        }

        public NavigationMenu NavigationMenu
        {
            get => _navigationMenu;
            set => this.RaiseAndSetIfChanged(ref _navigationMenu, value);
        }

        public NavigationItem NavigationMenuSelection
        {
            get => _navigationMenuSelection;
            set => this.RaiseAndSetIfChanged(ref _navigationMenuSelection, value);
        }

        public ExceptionViewModel ExceptionViewModel
        {
            get => _exceptionViewModel;
            set => this.RaiseAndSetIfChanged(ref _exceptionViewModel, value);
        }

        public AlbumsViewModel? AlbumsViewModel
        {
            get => _albumsViewModel;
            set => this.RaiseAndSetIfChanged(ref _albumsViewModel, value);
        }

        public AudioViewModelBase? CurrentAudioViewModel
        {
            get => _currentAudioViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentAudioViewModel, value);
        }

        public RepostViewModel? RepostViewModel
        {
            get => _repostViewModel;
            set => this.RaiseAndSetIfChanged(ref _repostViewModel, value);
        }

        public AddToAlbumViewModel? AddToAlbumViewModel
        {
            get => _addToAlbumViewModel;
            set => this.RaiseAndSetIfChanged(ref _addToAlbumViewModel, value);
        }

        /// <summary>
        /// Текущий аккаунт
        /// </summary>

        public SavedAccountModel CurrentAccountModel
        {
            get => _currentAccountModel;
            set => this.RaiseAndSetIfChanged(ref _currentAccountModel, value);
        }

        /// <summary>
        /// Видимость расширяемого текста меню
        /// </summary>

        public bool MenuTextIsVisible
        {
            get => _menuTextIsVisible;
            set => this.RaiseAndSetIfChanged(ref _menuTextIsVisible, value);
        }

        public int MenuSelectionIndex
        {
            get => _menuSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _menuSelectionIndex, value);
        }

        public GridLength MenuColumnWidth
        {
            get => _menuColumnWidth;
            set => this.RaiseAndSetIfChanged(ref _menuColumnWidth, value);
        }

        public bool ExceptionIsVisible { get; set; }

        public bool IsMaximized
        {
            get => _isMaximized;
            set => this.RaiseAndSetIfChanged(ref _isMaximized, value);
        }

        public ICommand AvatarPressedCommand { get; }

        public ICommand OpenHideMiniPlayerCommand { get; }
        public ICommand LogOutCommand { get; }


        private readonly IContainerProvider _containerProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IEventAggregator _eventAggregator;

        private CurrentMusicListViewModel? _currentMusicListViewModel;
        private AllMusicViewModel? _allMusicListViewModel;
        private AudioSearchViewModel? _searchViewModel;
        private RecomendationsViewModel? _recomendationsViewModel;

        private double _oldheight = 0;
        private bool _siderBarAnimationIsPlaying;
        private bool _menuIsOpen;
        private PlayerControlViewModel _playerContext;
        private AuthorizationViewModel _vkLoginViewModel;
        private NavigationMenu _navigationMenu;
        private SavedAccountModel _currentAccountModel;
        private GridLength _menuColumnWidth;
        private int _menuSelectionIndex;
        private bool _isMaximized;
        private bool _menuTextIsVisible;
        private AddToAlbumViewModel _addToAlbumViewModel;
        private NavigationItem _navigationMenuSelection;
        private ExceptionViewModel _exceptionViewModel;
        private AlbumsViewModel _albumsViewModel;
        private AudioViewModelBase _currentAudioViewModel;
        private RepostViewModel _repostViewModel;
    }
}