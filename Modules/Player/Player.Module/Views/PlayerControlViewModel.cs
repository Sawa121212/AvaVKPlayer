using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Common.Core.Views;
using Equalizer.Module.Views;
using ManagedBass;
using Player.Domain;
using Player.Domain.ETC;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Timer = System.Timers.Timer;

namespace Player.Module.Views
{
    public partial class PlayerControlViewModel : ViewModelBase
    {
        public delegate void OpenRepostWindowDelegate(AudioModel audioModel);

        public delegate void SetCollection(ObservableCollection<AudioModel> audioCollection, int selectedIndex);

        public delegate void AudioChanged(AudioModel? model);

        private static ObservableCollection<AudioModel>? _playList;
        private static ObservableCollection<AudioModel>? _allData;

        private static PlayerControlViewModel? _instance;

        private AudioModel _currentAudio;
        private bool _mute;
        private bool _pauseButtonIsVisible;
        private bool _playerIsPlaying = true;
        private int _playPosition = 0;
        private bool _repeat;
        private bool _shuffling;
        private bool _useEqualizer;
        private Task? _playTask;
        private readonly Timer _timer = new();
        private double _volume = 1;


        private PlayerControlViewModel()
        {
            Player = new Domain.Player();
            CurrentAudio = null;
            EqualizerViewModel = new EqualizerViewModel();
            OpenCloseEqualizer = ReactiveCommand.Create(() =>
            {
                if (EqualizerIsOpen)
                    EqualizerIsOpen = false;
                else EqualizerIsOpen = true;
            });

            PlayCommand = ReactiveCommand.Create(() =>
            {
                if (!Player.Play())
                    return;

                EqualizerViewModel.UpdateFx();
                OnUpdatePlayerStatus();
            });
            PauseCommand = ReactiveCommand.Create(() =>
            {
                if (Player.Pause())
                    OnUpdatePlayerStatus();
            });
            NextCommand = ReactiveCommand.Create(() => PlayNext());
            PreviousCommand = ReactiveCommand.Create(() => PlayPrevious());


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
            PlayerIsPlaying = !_playerIsPlaying;
        }

        private void PlayerControlViewModel_SetPlaylistEvent(ObservableCollection<AudioModel>? audioCollection,
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
            if (isEnd && !Repeat)
            {
                PlayNext();
                AutoNext = true;
            }
            else if (isEnd && Repeat)
            {
                Player.Update();
                Player.Play();
            }
        }

        private void PlayNext()
        {
            if (_playList != null)
            {
                List<AudioModel>? list = _playList.ToList();
                int index = list.IndexOf(CurrentAudio);
                if (index < list.Count - 1)
                {
                    CurrentAudio = list[index + 1];
                    AudioChangedEvent?.Invoke(_currentAudio);
                }
            }
        }

        private void PlayPrevious()
        {
            if (_playList != null)
            {
                List<AudioModel>? list = _playList.ToList();
                int index = list.IndexOf(CurrentAudio);
                if (index > 0)
                {
                    CurrentAudio = list[index - 1];
                    AudioChangedEvent?.Invoke(_currentAudio);
                }
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
                if (Volume == 0) Mute = true;
                else Mute = false;
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

        public Domain.Player Player { get; set; }

        public bool PlayerIsPlaying
        {
            get => _playerIsPlaying;
            set => this.RaiseAndSetIfChanged(ref _playerIsPlaying, value);
        }

        public CancellationToken CancellationToken { get; private set; } = new CancellationToken();
        public bool IsBusy { get; set; }

        private bool AutoNext { get; set; }

        public static PlayerControlViewModel Instance =>
            _instance is null ? _instance = new PlayerControlViewModel() : _instance;

        public static event SetCollection? SetPlaylistEvent;

        public static event OpenRepostWindowDelegate? OpenRepostWindowEvent;


        public event AudioChanged AudioChangedEvent;
        public EqualizerViewModel EqualizerViewModel { get; set; }


        [Reactive] public bool EqualizerIsOpen { get; set; }

        public IReactiveCommand PlayCommand { get; set; }
        public IReactiveCommand PauseCommand { get; set; }

        public IReactiveCommand NextCommand { get; set; }

        public IReactiveCommand PreviousCommand { get; set; }

        public IReactiveCommand RepeatToggleCommand { get; set; }
        public IReactiveCommand MuteToggleCommand { get; set; }

        public IReactiveCommand ShuffleToogleCommand { get; set; }
        public IReactiveCommand RepostCommand { get; set; }
        public IReactiveCommand OpenCloseEqualizer { get; set; }
    }
}