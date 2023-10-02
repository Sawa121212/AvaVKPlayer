using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Layout;
using Notification.Module.Services;
using Prism.Commands;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkPlayer.Module.ViewModels.Base;
using VkPlayer.Module.Views;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels.Audios
{
    public sealed class AllMusicViewModel : AudioViewModelBase
    {
        public AllMusicViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
            // ToDo Events.AudioAddEvent += Events_AudioAddEvent;
            // ToDo Events.AudioRemoveEvent += Events_AudioRemoveEvent;
            StartSearchObservable(new TimeSpan(0, 0, 0, 1, 0));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioAddIsVisible = false;
            GoToArtistCommand = new DelegateCommand<AudioModel>(OnGoToArtist);
        }

        private void Events_AudioRemoveEvent(AudioModel audioModel)
        {
            AllDataCollection?.Remove(audioModel);
            DataCollection = AllDataCollection;
        }

        private void Events_AudioAddEvent(AudioModel audioModel)
        {
            AllDataCollection?.Insert(0, audioModel);
            DataCollection = AllDataCollection;
        }

        /// <inheritdoc />
        public override void Search(string? text)
        {
            if (_searching == true)
            {
                //cancellationTokenSource?.TryReset();
                _searching = false;
                Search(text);
            }
            else
            {
                _searching = true;
                Task.Run(() =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            if (PlayerControlViewModel.Instance?.CurrentAudio != null)
                                SelectToModel(PlayerControlViewModel.Instance.CurrentAudio, true);
                            DataCollection = AllDataCollection;
                            Offset = DataCollection.Count();
                            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                                StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical));
                        }
                        else
                        {
                            Offset = 0;
                            IsLoading = true;
                            StopScrollChandegObserVable();


                            DataCollection = new ObservableCollection<AudioModel>();
                            while (true)
                            {
                                try
                                {
                                    VkCollection<Audio>? res = VkApiManager.GetAudio(new AudioGetParams
                                    {
                                        Offset = Offset,
                                        Count = 500,
                                    });

                                    if (res != null && res.Count > 0)
                                    {
                                        IEnumerable<Audio>? searchRes = res.Where(x =>
                                            x.Title.ToLower().Contains(text.ToLower()) ||
                                            x.Artist.ToLower().Contains(text.ToLower())).Distinct();

                                        DataCollection.AddRange(searchRes);
                                        ResponseCount = res.Count;
                                        Offset += res.Count;
                                    }
                                    else break;
                                }
                                catch (Exception ex)
                                {
                                    break;
                                }
                            }

                            DataCollection.StartLoadImagesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        DataCollection = AllDataCollection;
                        SearchText = "";
                    }
                    finally
                    {
                        IsLoading = false;
                        _searching = false;
                    }
                }, _cancellationTokenSource.Token);
            }
        }


        /// <inheritdoc />
        public override void OnSelectedItem()
        {
            if (CurrentAudio == SelectedItem || DataCollection == null || SelectedItem == null)
            {
                return;
            }

            int index = DataCollection.IndexOf(SelectedItem);

            if (SelectedItem.IsNotAvailable)
            {
                _notificationService.Show("Ошибка",
                    $"Музыка \" {SelectedItem.Artist} - {SelectedItem.Title}\" не доступна", NotificationType.Warning);

                // если музыка не доступна, то запускаем следующую музыку
                index = DataCollection.IndexOf(SelectedItem);
                if (DataCollection.Count > index + 1)
                {
                    index++;
                    SelectedItem = DataCollection[index];
                }
                else
                {
                    return;
                }
            }

            PlayerControlViewModel.SetPlaylist(new ObservableCollection<AudioModel>(DataCollection.ToList()),
                index);

            CurrentAudio = SelectedItem;
            SelectToModel(SelectedItem, false);
        }


        /// <inheritdoc />
        protected override void LoadData()
        {
            VkCollection<Audio>? res = VkApiManager.GetAudio(new AudioGetParams
            {
                Count = 500,
                Offset = (uint) Offset
            });

            if (res != null)
            {
                DataCollection.AddRange(res);
                Task.Run(() => { DataCollection.StartLoadImages(); });
                Offset += res.Count;

                ResponseCount = res.Count;
            }

            AllDataCollection = DataCollection;
        }

        /// <inheritdoc />
        public override void OnSelected(AudioModel item)
        {
        }

        /// ToDo: OnGoToArtist
        private void OnGoToArtist(AudioModel audioModel)
        {
            if (audioModel?.Artist == null)
            {
                return;
            }

            /*MenuSelectionIndex = 3;
            if (PlayerContext?.CurrentAudio != null)
            {
                CurrentAudioViewModel.SelectToModel(PlayerContext?.CurrentAudio, false);
                CurrentAudioViewModel.SelectedIndex = -1;
            }

            if (_searchViewModel == null)
                return;

            _searchViewModel.IsLoading = true;
            _searchViewModel.SearchText = tb.Artist;*/
        }

        /// <summary>
        /// Текущая выбранная музыка
        /// </summary>
        private AudioModel CurrentAudio
        {
            get => _currentAudio;
            set => this.RaiseAndSetIfChanged(ref _currentAudio, value);
        }

        private readonly INotificationService _notificationService;
        CancellationTokenSource _cancellationTokenSource = new();
        bool _searching = false;
        private AudioModel _currentAudio;

        public ICommand GoToArtistCommand { get; }
    }
}