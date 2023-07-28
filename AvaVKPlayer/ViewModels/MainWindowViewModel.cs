using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.ViewModels.Audios;
using AvaVKPlayer.ViewModels.Audios.Albums;
using AvaVKPlayer.ViewModels.Base;
using AvaVKPlayer.ViewModels.Exceptions;
using AvaVKPlayer.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Exception;

namespace AvaVKPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private double _oldheight = 0;
        private bool _siderBarAnimationIsPlaying;
        private bool _menuIsOpen;

        private CurrentMusicListViewModel? _currentMusicListViewModel;
        private AllMusicViewModel? _allMusicListViewModel;
        private AudioSearchViewModel? _searchViewModel;
        private RecomendationsViewModel? _recomendationsViewModel;

        public MainWindowViewModel()
        {
            MenuColumnWidth = new GridLength(50);
            IsMaximized = true;
            PlayerContext = PlayerControlViewModel.Instance;
            PlayerContext.AudioChangedEvent += PlayerContext_AudioChangedEvent;

            Events.AudioRepostEvent += Events_AudioRepostEvent;
            Events.AudioAddToAlbumEvent += Events_AudioAddToAlbumEvent;
            ExceptionViewModel.ViewExitEvent += ExceptionViewModel_ViewExitEvent;

            Events.VkApiChanged += StaticObjects_VkApiChanged;

            VkLoginViewModel = new LoginControlViewModel();

            OpenHideMiniPlayerCommand = ReactiveCommand.Create(() =>
            {
                if (IsMaximized)
                {
                    IsMaximized = false;
                    _oldheight = MainWindow.Instance.Height;
                    MenuColumnWidth = new GridLength(0);
                    MainWindow.Instance.Topmost = true;
                    MainWindow.Instance.MinHeight = 100;
                    MainWindow.Instance.Height = 100;
                    MainWindow.Instance.MaxHeight = 100;
                }
                else
                {
                    IsMaximized = true;
                    if (_menuIsOpen)
                    {
                        MenuColumnWidth = new GridLength(200);
                    }
                    else MenuColumnWidth = new GridLength(60);

                    MainWindow.Instance.Topmost = false;
                    MainWindow.Instance.MaxHeight = Double.PositiveInfinity;
                    MainWindow.Instance.MinHeight = 400;
                    MainWindow.Instance.Height = _oldheight;
                }
            });

            InvokeHandler.TaskErrorResponsedEvent += (handlerObject, exception) =>
            {
                if (exception is UserAuthorizationFailException)
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
                }
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

            this.WhenAnyValue(vm => vm.MenuSelectionIndex).Subscribe(value => OpenViewFromMenu(value));
        }

        private void Events_AudioAddToAlbumEvent(AudioModel audiomodel)
        {
            AddToAlbumViewModel = new AddToAlbumViewModel(audiomodel);
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
                            AlbumsViewModel = new OpenAlbumViewModel();
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
                    CurrentAccountModel = GlobalVars.CurrentAccount;
                    MenuSelectionIndex = 1;

                    if (CurrentAccountModel.Image is null)
                        CurrentAccountModel.LoadAvatar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EErr");
                }
            });

            Events.VkApiChanged -= StaticObjects_VkApiChanged;
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

            Events.VkApiChanged += StaticObjects_VkApiChanged;
        }

        private void ExceptionViewModel_ViewExitEvent()
        {
            CurrentAudioViewModel.IsLoading = true;
            CurrentAudioViewModel.IsError = false;
            ExceptionIsVisible = false;
        }


        public PlayerControlViewModel PlayerContext { get; set; }

        public LoginControlViewModel? VkLoginViewModel { get; set; }

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