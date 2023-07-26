﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using ReactiveUI.Fody.Helpers;

namespace AvaVKPlayer.ViewModels.Base
{
    public abstract class AudioViewModelBase : DataViewModelBase<AudioModel>
    {
        public static Action? LoadMusicsAction { get; set; }
        public AudioListButtonsViewModel AudioListButtons { get; set; }

        [Reactive]
        public bool ScrollToItem { get; set; } = false;


        public AudioViewModelBase()
        {
            SearchIsVisible = true;
            AudioListButtons = new AudioListButtonsViewModel();
            LoadMusicsAction = () =>
            {
                if (string.IsNullOrEmpty(_SearchText))
                    if (ResponseCount > 0 && IsLoading is false)
                        InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));
            };

        }

        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {

            if (args.Source is TextBlock)
            {
                return;
            }

            var model = args?.GetContent<AudioModel>();

            if (model != null)
            {
                var index = DataCollection?.IndexOf(model) ?? -1;

                if (index > -1)
                {
                    
                    PlayerControlViewModel.SetPlaylist(
                        new ObservableCollection<AudioModel>(DataCollection.Cast<AudioModel>().ToList()),
                        index);

                    SelectToModel(model, false);
                }
            }

        }
        public override void Search(string? text)
        {
            try
            {

                if (string.IsNullOrEmpty(text))
                {
                    SelectedIndex = -1;
                    DataCollection = _AllDataCollection;
                    StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

                }
                else if (_AllDataCollection != null && _AllDataCollection.Count() > 0)
                {
                    StopScrollChandegObserVable();

                    var searchRes = _AllDataCollection.Where(x =>
                            x.Title.ToLower().Contains(text.ToLower()) ||
                            x.Artist.ToLower().Contains(text.ToLower()))
                        .Distinct();
                    DataCollection = new ObservableCollection<AudioModel>(searchRes);
                }

                DataCollection.StartLoadImagesAsync();
            }
            catch (Exception ex)
            {
                DataCollection = _AllDataCollection;
                SearchText = "";
            }
        }
        public void SelectToModel(AudioModel? model,bool scrolled)
        {
            if (model == null)
                return;

            int index = GetIndexFromAudio(model);
            if (index > -1)
            {
               
                ScrollToItem = scrolled;
                SelectedIndex = 0;
                SelectedIndex = index;
                ScrollToItem = false;

            }

        }

      
        public int GetIndexFromAudio(AudioModel model)
        {
            return DataCollection.FindIndex(x => x.ID == model.ID);
        } 
    }





}