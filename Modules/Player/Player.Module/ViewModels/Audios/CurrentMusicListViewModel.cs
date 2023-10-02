using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Input;
using VkPlayer.Domain;
using VkPlayer.Module.ViewModels.Base;
using VkPlayer.Module.Views;

namespace VkPlayer.Module.ViewModels.Audios
{
    public class CurrentMusicListViewModel : AudioViewModelBase
    {
        public CurrentMusicListViewModel() : base()
        {
            SearchIsVisible = false;
            IsLoading = false;
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
            AudioListButtons.AudioRemoveIsVisible = false;
            AudioListButtons.AudioAddIsVisible = false;
            AudioListButtons.AudioAddToAlbumIsVisible = false;
        }

        /// <inheritdoc />
        public override void OnSelected(AudioModel item)
        {
        }

        public override void OnSelectedItem(object sender, PointerPressedEventArgs args)
        {
            PlayerControlViewModel.SetPlaylistEvent -= PlayerControlViewModelOnSetPlaylistEvent;
            base.OnSelectedItem(sender, args);
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
        }


        private void PlayerControlViewModelOnSetPlaylistEvent(
            IEnumerable<AudioModel> audiocollection, int selectedindex)
        {
            if (DataCollection?.AsEnumerable() != audiocollection)
            {
                DataCollection = new ObservableCollection<AudioModel>();
                DataCollection.AddRange(audiocollection);
                AllDataCollection = DataCollection;
            }

            SelectedIndex = selectedindex;
        }
    }
}