using System;
using System.Linq;
using Avalonia.Input;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.Notify;
using AvaVKPlayer.ViewModels.Interfaces;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels.Audios.Albums
{
    public class AddToAlbumViewModel : 
        AlbumsViewModel,ICloseView
    {
        private AudioModel _audioModel;
        public AddToAlbumViewModel(AudioModel audioModel)
        {
            if (audioModel is null)
            {
                Notify.NotifyManager.Instance.PopMessage(
                    new NotifyData("Ошибка добавления",$"Аудиозапись не выбрана"));
                throw new ArgumentNullException(nameof(audioModel));
            }
            else
            {
                this._audioModel = audioModel;
                CloseCommand = ReactiveCommand.Create(() => CloseViewEvent?.Invoke());
            }
        }
        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            AudioAlbumModel? item = args?.GetContent<AudioAlbumModel>();
            if (item != null)
            {
                try
                {
                    string[]? ids = new[] { _audioModel.GetAudioIdFormatWithAccessKey() };
              
                    GlobalVars.VkApi.Audio.AddToPlaylist(item.OwnerId, item.Id, ids);
                
                    Notify.NotifyManager.Instance.PopMessage(
                        new NotifyData("Успешно добавлено",$"Аудиозапись {_audioModel.Title} добавлена в альбом {item.Title}"));
                }
                catch (Exception ex)
                {
                    Notify.NotifyManager.Instance.PopMessage(
                        new NotifyData("Ошибка добавления",$"Аудиозапись {_audioModel.Title} не добавлена в альбом {item.Title}"));
                }
            }
            CloseViewEvent?.Invoke();
        }
        protected override void LoadData()
        {
            if (GlobalVars.CurrentAccount?.UserId != null)
            {
                VkCollection<AudioPlaylist>? res = 
                    GlobalVars.VkApi.Audio.GetPlaylists((long)GlobalVars.CurrentAccount.UserId, 200,
                        (uint)Offset);
                if (res != null)
                {
                    DataCollection.AddRange(res.Where(x=>x.Original == null));
                    DataCollection.StartLoadImagesAsync();
                }
            }
        }

        public event ICloseView.CloseViewDelegate? CloseViewEvent;
        public IReactiveCommand CloseCommand { get; set; }
    }
}