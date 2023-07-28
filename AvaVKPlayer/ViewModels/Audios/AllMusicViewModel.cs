using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Layout;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.ViewModels.Base;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels.Audios
{
    public sealed class AllMusicViewModel : AudioViewModelBase
    {
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        bool _searching = false;

        public AllMusicViewModel()

        {
            Events.AudioAddEvent += Events_AudioAddEvent;
            Events.AudioRemoveEvent += Events_AudioRemoveEvent;
            StartSearchObservable(new TimeSpan(0, 0, 0, 1, 0));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioAddIsVisible = false;
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
                                    VkCollection<Audio>? res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
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


        protected override void LoadData()
        {
            VkCollection<Audio>? res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
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
    }
}