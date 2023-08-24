using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Layout;
using Common.Core.ToDo;
using ReactiveUI;
using VkPlayer.Domain;
using VkPlayer.Domain.Base;
using VkPlayer.Domain.ETC;

namespace VkPlayer.Module.ViewModels.Base
{
    public abstract class AudioViewModelBase : DataViewModelBase<AudioModel>
    {
        public AudioViewModelBase()
        {
            SearchIsVisible = true;
            AudioListButtons = new AudioListButtonsViewModel();
            LoadMusicsAction = () =>
            {
                if (!string.IsNullOrEmpty(SearchText))
                    return;

                if (ResponseCount > 0 && IsLoading is false)
                {
                    InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));
                }
            };
        }

        /// <inheritdoc />
        public override void OnSelectedItem()
        {
        }

        /// <inheritdoc />
        public override void Search(string? text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    SelectedIndex = -1;
                    DataCollection = AllDataCollection;
                    StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
                }
                else if (AllDataCollection != null && AllDataCollection.Count() > 0)
                {
                    StopScrollChandegObserVable();

                    IEnumerable<AudioModel>? searchRes = AllDataCollection.Where(x =>
                            x.Title.ToLower().Contains(text.ToLower()) ||
                            x.Artist.ToLower().Contains(text.ToLower()))
                        .Distinct();
                    DataCollection = new ObservableCollection<AudioModel>(searchRes);
                }

                DataCollection.StartLoadImagesAsync();
            }
            catch (Exception ex)
            {
                DataCollection = AllDataCollection;
                SearchText = "";
            }
        }

        public void SelectToModel(AudioModel? model, bool scrolled)
        {
            if (model == null)
                return;

            int index = GetIndexFromAudio(model);

            if (index <= -1)
                return;

            ScrollToItem = scrolled;
            SelectedIndex = index;
            ScrollToItem = false;
        }


        private int GetIndexFromAudio(AudioModel model)
        {
            return DataCollection?.FindIndex(x => x.Id == model.Id) ?? -1;
        }


        public static Action? LoadMusicsAction { get; set; }

        public AudioListButtonsViewModel AudioListButtons
        {
            get => _audioListButtons;
            set => this.RaiseAndSetIfChanged(ref _audioListButtons, value);
        }

        public bool ScrollToItem
        {
            get => _scrollToItem;
            set => this.RaiseAndSetIfChanged(ref _scrollToItem, value);
        }

        private AudioListButtonsViewModel _audioListButtons;
        private bool _scrollToItem;
    }
}