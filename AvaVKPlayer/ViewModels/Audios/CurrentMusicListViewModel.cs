using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Input;
using AvaVKPlayer.Models;
using AvaVKPlayer.ViewModels.Base;
using DynamicData;

namespace AvaVKPlayer.ViewModels.Audios
{
    public class CurrentMusicListViewModel : AudioViewModelBase
    {
        public CurrentMusicListViewModel()
        {
            
            SearchIsVisible = false;
            IsLoading = false;
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
            AudioListButtons.AudioRemoveIsVisible = false;
            AudioListButtons.AudioAddIsVisible = false;
            AudioListButtons.AudioAddToAlbumIsVisible = false;
        }

        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            PlayerControlViewModel.SetPlaylistEvent -= PlayerControlViewModelOnSetPlaylistEvent;
            base.SelectedItem(sender, args);
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
        }
        

        private void PlayerControlViewModelOnSetPlaylistEvent(IEnumerable<AudioModel> audiocollection,
            int selectedindex)
        {
           
            DataCollection = new ObservableCollection<AudioModel>();
            DataCollection.AddRange(audiocollection);
            AllDataCollection = DataCollection;
            SelectedIndex = selectedindex;
            
        }
    }
}