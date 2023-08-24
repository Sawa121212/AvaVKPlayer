using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Common.Core.Views;
using Equalizer.Module.Views;
using ManagedBass;
using Prism.Commands;
using ReactiveUI;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using Timer = System.Timers.Timer;

namespace VkPlayer.Module.Views
{
    public partial class PlayerControlViewModel : ViewModelBase
    {
        public PlayerControlViewModel()
        {
            Player = new Player();
            CurrentAudio = null;
            EqualizerViewModel = new EqualizerControlViewModel();
            OpenCloseEqualizer = ReactiveCommand.Create(() => { EqualizerIsOpen = !EqualizerIsOpen; });

            PlayCommand = new DelegateCommand(OnPlay);
            PauseCommand = new DelegateCommand(OnPause);
            NextCommand = new DelegateCommand(PlayNext);
            PreviousCommand = new DelegateCommand(PlayPrevious);


            RepeatToggleCommand = ReactiveCommand.Create(() => { Repeat = !Repeat; });
            ShuffleToogleCommand = ReactiveCommand.Create(() => { Shuffling = !Shuffling; });

            MuteToggleCommand = ReactiveCommand.Create(() =>
            {
                if (Mute)
                {
                    if (Volume == 0)
                        return;
                    Mute = false;
                    Player.SetVolume(Volume);
                }
                else
                {
                    Mute = true;
                    Player.SetVolume(0);
                }
            });

            //ToDo RepostCommand = ReactiveCommand.Create(() => Events.AudioRepostEventCall(CurrentAudio));
            _timer.Interval = 1000;
            _timer.Elapsed += _Timer_Elapsed;
            SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
        }

        public void EqualizerElement_OnLostFocus(object? sender, RoutedEventArgs e)
        {
            EqualizerIsOpen = false;
        }

        public void EqualizerElement_OnLosPointer(object? sender, PointerEventArgs e)
        {
            EqualizerIsOpen = false;
        }

        public void VolumeChanged(object sender, PointerCaptureLostEventArgs e)
        {
            Slider s = e.Source as Slider;
            if (s != null)
                Player.SetPositon(s.Value);
        }

        public static void SetPlaylist(ObservableCollection<AudioModel> audioCollection, int selectedIndex)
        {
            SetPlaylistEvent?.Invoke(audioCollection, selectedIndex);
        }

        private void OnUpdatePlayerStatus()
        {
            IsPlaying = !IsPlaying;
        }

        private void PlayerControlViewModel_SetPlaylistEvent(
            ObservableCollection<AudioModel>? audioCollection,
            int selectedIndex)
        {
            if (audioCollection == null)
                return;

            _playList = audioCollection;
            _allData = _playList;
            CurrentAudio = audioCollection.Any() ? audioCollection.ElementAt(selectedIndex) : null;
        }

        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() => PlayPosition = Player.GetPositionSeconds());

            bool isEnd = (PlayPosition == CurrentAudio.Duration)
                         || (Player.GetStatus() == PlaybackState.Stopped);

            switch (isEnd)
            {
                case true when !Repeat:
                    PlayNext();
                    AutoNext = true;
                    break;
                case true when Repeat:
                    Player.Update();
                    Player.Play();
                    break;
            }
        }

        private void PlayNext()
        {
            if (_playList == null)
            {
                return;
            }

            List<AudioModel>? list = _playList.ToList();
            int index = list.IndexOf(CurrentAudio);
            if (index >= list.Count - 1)
            {
                return;
            }

            CurrentAudio = list[index + 1];
            AudioChangedEvent?.Invoke(_currentAudio);
        }

        private void PlayPrevious()
        {
            if (_playList == null)
            {
                return;
            }

            List<AudioModel>? list = _playList.ToList();
            int index = list.IndexOf(CurrentAudio);

            if (index <= 0)
            {
                return;
            }

            CurrentAudio = list[index - 1];
            AudioChangedEvent?.Invoke(_currentAudio);
        }

        private void OnPlay()
        {
            if (IsPlaying)
            {
                OnPause();
                return;
            }

            if (!Player.Play())
            {
                return;
            }

            EqualizerViewModel.UpdateFx();
            OnUpdatePlayerStatus();
        }

        private void OnPause()
        {
            if (Player.Pause())
            {
                OnUpdatePlayerStatus();
            }
        }

        public bool Repeat
        {
            get => _repeat;
            set => this.RaiseAndSetIfChanged(ref _repeat, value);
        }


        public bool Shuffling
        {
            get => _shuffling;
            set
            {
                SetPlaylistEvent -= PlayerControlViewModel_SetPlaylistEvent;
                this.RaiseAndSetIfChanged(ref _shuffling, value);

                if (_shuffling)
                {
                    _allData = _playList;
                    _playList = _allData.Shuffle();
                }
                else
                {
                    _playList = _allData;
                }

                SetPlaylist(_playList, 0);
                SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
            }
        }

        public bool Mute
        {
            get => _mute;
            set => this.RaiseAndSetIfChanged(ref _mute, value);
        }

        public int PlayPosition
        {
            get => _playPosition;
            set => this.RaiseAndSetIfChanged(ref _playPosition, value);
        }

        public double Volume
        {
            get => _volume;
            set
            {
                this.RaiseAndSetIfChanged(ref _volume, value);
                Mute = Volume == 0;
                Player.SetVolume(_volume);
            }
        }

        public AudioModel CurrentAudio
        {
            get => _currentAudio;
            set
            {
                try
                {
                    PlayPosition = 0;

                    _timer?.Stop();
                    Player.Stop();
                    if (_playTask != null)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                    }

                    if (value is null)
                    {
                        this.RaiseAndSetIfChanged(ref _currentAudio, new AudioModel()
                        {
                            Duration = 0,
                        });
                        return;
                    }
                    else
                    {
                        this.RaiseAndSetIfChanged(ref _currentAudio, value);
                    }


                    if (_currentAudio.IsNotAvailable)
                    {
                        //Notify.NotifyManager.Instance.PopMessage(new NotifyData("Ошибка",
                        //    $"Аудиозапись {_currentAudio.Artist} - {_currentAudio.Title} не доступна"));
                        if (AutoNext)
                        {
                            AutoNext = false;
                            PlayNext();
                        }

                        return;
                    }

                    OnUpdatePlayerStatus();


                    _timer?.Start();

                    _playTask = new Task(() =>
                    {
                        if (IsBusy) return;

                        IsBusy = true;
                        if (Player.Play(_currentAudio))
                        {
                            Player.SetVolume(Volume);
                            EqualizerViewModel.UpdateEqualizer();
                        }

                        IsBusy = false;
                    }, cancellationToken: CancellationToken);

                    _playTask.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        public Player Player
        {
            get => _player;
            set => this.RaiseAndSetIfChanged(ref _player, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
        }

        public CancellationToken CancellationToken { get; private set; } = new();

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        private bool AutoNext
        {
            get => _autoNext;
            set => this.RaiseAndSetIfChanged(ref _autoNext, value);
        }

        public static PlayerControlViewModel Instance =>
            _instance is null ? _instance = new PlayerControlViewModel() : _instance;

        public static event SetCollection? SetPlaylistEvent;

        public static event OpenRepostWindowDelegate? OpenRepostWindowEvent;


        public event AudioChanged AudioChangedEvent;

        public EqualizerControlViewModel EqualizerViewModel
        {
            get => _equalizerViewModel;
            set => this.RaiseAndSetIfChanged(ref _equalizerViewModel, value);
        }

        public bool EqualizerIsOpen
        {
            get => _equalizerIsOpen;
            set => this.RaiseAndSetIfChanged(ref _equalizerIsOpen, value);
        }

        public delegate void OpenRepostWindowDelegate(AudioModel audioModel);

        public delegate void SetCollection(ObservableCollection<AudioModel> audioCollection, int selectedIndex);

        public delegate void AudioChanged(AudioModel? model);

        private static ObservableCollection<AudioModel>? _playList;
        private static ObservableCollection<AudioModel>? _allData;

        private static PlayerControlViewModel? _instance;

        private AudioModel _currentAudio;
        private bool _mute;
        private bool _pauseButtonIsVisible;
        private int _playPosition = 0;
        private bool _repeat;
        private bool _shuffling;
        private bool _useEqualizer;
        private Task? _playTask;
        private readonly Timer _timer = new();
        private double _volume = 1;
        private Player _player;
        private bool _isPlaying;
        private bool _isBusy;
        private bool _autoNext;
        private EqualizerControlViewModel _equalizerViewModel;
        private bool _equalizerIsOpen;


        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand RepeatToggleCommand { get; set; }
        public ICommand MuteToggleCommand { get; set; }

        public ICommand ShuffleToogleCommand { get; set; }
        public ICommand RepostCommand { get; set; }
        public ICommand OpenCloseEqualizer { get; set; }
    }
}