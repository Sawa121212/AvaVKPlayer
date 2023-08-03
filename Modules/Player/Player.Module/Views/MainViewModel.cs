using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Authorization.Module.Domain;
using Authorization.Module.Services;
using Authorization.Module.Views;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Common.Core.ToDo;
using Common.Core.Views;
using Player.Domain;
using Player.Domain.ETC;
using Player.Module.ViewModels.Audios;
using Player.Module.ViewModels.Audios.Albums;
using Player.Module.ViewModels.Base;
using Player.Module.ViewModels.Exceptions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Player.Module.Views
{
    public class MainViewModel : ViewModelBase
    {
        private double _oldheight = 0;
        private bool _siderBarAnimationIsPlaying;
        private bool _menuIsOpen;
        private readonly IAuthorizationService _authorizationService;
        private CurrentMusicListViewModel? _currentMusicListViewModel;
        private AllMusicViewModel? _allMusicListViewModel;
        private AudioSearchViewModel? _searchViewModel;
        private RecomendationsViewModel? _recomendationsViewModel;

        public MainViewModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            MenuColumnWidth = new GridLength(50);
            IsMaximized = true;

            PlayerContext = PlayerControlViewModel.Instance;

            PlayerContext.AudioChangedEvent += PlayerContext_AudioChangedEvent;
            //ToDo Events.AudioRepostEvent += Events_AudioRepostEvent;
            //ToDo Events.AudioAddToAlbumEvent += Events_AudioAddToAlbumEvent;
            ExceptionViewModel.ViewExitEvent += ExceptionViewModel_ViewExitEvent;
            //ToDo Events.VkApiChanged += StaticObjects_VkApiChanged;

            VkLoginViewModel = new AuthorizationViewModel(_authorizationService);

            // ToDo in ShellView
            /*OpenHideMiniPlayerCommand = ReactiveCommand.Create(() =>
            {
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
                }
            });*/

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

            this.WhenAnyValue(vm => vm.MenuSelectionIndex).Subscribe(OpenViewFromMenu);
        }

        private void Events_AudioAddToAlbumEvent(AudioModel audiomodel)
        {
            AddToAlbumViewModel = new AddToAlbumViewModel(audiomodel, _authorizationService);
            AddToAlbumViewModel.StartLoad();
            AddToAlbumViewModel.CloseViewEvent += AddToAlbumViewModel_CloseViewEvent;
            AddToAlbumIsVisible = true;
        }

        private void AddToAlbumViewModel_CloseViewEvent()
        {
            AddToAlbumViewModel.CloseViewEvent -= AddToAlbumViewModel_CloseViewEvent;
            AddToAlbumViewModel?.DataCollection?.Clear();
            AddToAlbumViewModel = null;
            AddToAlbumIsVisible = false;
        }

        private void PlayerContext_AudioChangedEvent(AudioModel? model)
        {
            CurrentAudioViewModel.SelectToModel(model, true);
        }

        private void Events_AudioRepostEvent(AudioModel audioModel)
        {
            RepostViewModel = new RepostViewModel(RepostToType.Friend, audioModel);
            RepostViewModel.CloseViewEvent += RepostViewModel_CloseViewEvent;
            RepostViewIsVisible = true;
        }

        private void RepostViewModel_CloseViewEvent()
        {
            RepostViewModel.CloseViewEvent -= RepostViewModel_CloseViewEvent;
            RepostViewModel?.DataCollection?.Clear();
            RepostViewIsVisible = false;
            RepostViewModel = null;
        }

        public void ArtistClicked(object sender, PointerPressedEventArgs e)
        {
            AudioModel? tb = e.GetContent<AudioModel>();
            if (tb?.Artist != null)
            {
                MenuSelectionIndex = 3;
                if (PlayerContext?.CurrentAudio != null)
                {
                    CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, false);
                    CurrentAudioViewModel.SelectedIndex = -1;
                }

                if (_searchViewModel == null)
                    return;

                _searchViewModel.IsLoading = true;
                _searchViewModel.SearchText = tb.Artist;
            }
        }

        public void OpenViewFromMenu(int menuIndex)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ExceptionIsVisible = false;
                CurrentAudioViewIsVisible = true;
                CurrentAudioViewModel = null;
                AlbumsIsVisible = false;

                switch (menuIndex)
                {
                    case 0:
                    {
                        CurrentAudioViewModel = _currentMusicListViewModel;
                        CurrentAudioViewModel?.SelectToModel(PlayerContext?.CurrentAudio, true);

                        break;
                    }
                    case 1:
                    {
                        if (_allMusicListViewModel == null)
                        {
                            _allMusicListViewModel = new AllMusicViewModel();
                            _allMusicListViewModel.StartLoad();
                        }

                        CurrentAudioViewModel = _allMusicListViewModel;
                        CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, true);

                        break;
                    }
                    case 2:
                    {
                        if (AlbumsViewModel == null)
                        {
                            AlbumsViewModel = new OpenAlbumViewModel(_authorizationService);
                            AlbumsViewModel.StartLoad();
                        }

                        CurrentAudioViewIsVisible = false;
                        AlbumsIsVisible = true;
                        break;
                    }
                    case 3:
                    {
                        CurrentAudioViewModel = _searchViewModel;
                        CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, true);


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


                        break;
                    }
                    case 5:
                    {
                        AccountExit();
                        break;
                    }
                }
            });
        }

        private void StaticObjects_VkApiChanged()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    _currentMusicListViewModel = new CurrentMusicListViewModel();
                    VkLoginIsVisible = false;
                    CurrentAccountModel = _authorizationService.CurrentAccount;
                    MenuSelectionIndex = 1;

                    if (CurrentAccountModel.Image is null)
                        CurrentAccountModel.LoadAvatar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EErr");
                }
            });

            //ToDo Events.VkApiChanged -= StaticObjects_VkApiChanged;
        }

        private void AccountExit()
        {
            try
            {
                PlayerContext.CurrentAudio = null;
            }
            catch (Exception ex)
            {
            }

            CurrentAudioViewIsVisible = false;
            AlbumsIsVisible = false;
            RepostViewIsVisible = false;
            ExceptionIsVisible = false;
            VkLoginIsVisible = true;

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

            //ToDo Events.VkApiChanged += StaticObjects_VkApiChanged;
        }

        private void ExceptionViewModel_ViewExitEvent()
        {
            CurrentAudioViewModel.IsLoading = true;
            CurrentAudioViewModel.IsError = false;
            ExceptionIsVisible = false;
        }


        public PlayerControlViewModel PlayerContext { get; set; }

        public AuthorizationViewModel? VkLoginViewModel { get; set; }

        [Reactive] public ExceptionViewModel ExceptionViewModel { get; set; }

        [Reactive] public AlbumsViewModel? AlbumsViewModel { get; set; }

        [Reactive] public AudioViewModelBase? CurrentAudioViewModel { get; set; }

        [Reactive] public RepostViewModel? RepostViewModel { get; set; }
        [Reactive] public AddToAlbumViewModel? AddToAlbumViewModel { get; set; }

        [Reactive] public SavedAccountModel CurrentAccountModel { get; set; }

        [Reactive] public bool MenuTextIsVisible { get; set; }

        [Reactive] public bool AlbumsIsVisible { get; set; }

        [Reactive] public bool RepostViewIsVisible { get; set; }

        [Reactive] public bool AddToAlbumIsVisible { get; set; }

        [Reactive] public bool CurrentAudioViewIsVisible { get; set; }

        [Reactive] public bool VkLoginIsVisible { get; set; } = true;

        [Reactive] public int MenuSelectionIndex { get; set; }

        [Reactive] public GridLength MenuColumnWidth { get; set; }

        [Reactive] public bool ExceptionIsVisible { get; set; }

        [Reactive] public bool IsMaximized { get; set; }

        [Reactive] public ICommand AvatarPressedCommand { get; set; }

        [Reactive] public ICommand OpenHideMiniPlayerCommand { get; set; }
    }
}