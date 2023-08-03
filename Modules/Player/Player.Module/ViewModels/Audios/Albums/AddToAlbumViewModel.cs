using System;
using System.Linq;
using Authorization.Module.Services;
using Avalonia.Input;
using Common.Core.Views.Interfaces;
using Player.Domain;
using Player.Domain.ETC;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;
using VkProvider.Module;

namespace Player.Module.ViewModels.Audios.Albums
{
    public class AddToAlbumViewModel : AlbumsViewModel, ICloseView
    {
        private AudioModel _audioModel;
        private readonly IAuthorizationService _authorizationService;

        public AddToAlbumViewModel(AudioModel audioModel, IAuthorizationService authorizationService) : base(authorizationService)
        {
            _authorizationService = authorizationService;
            if (audioModel is null)
            {
                //Notify.NotifyManager.Instance.PopMessage(
                //    new NotifyData("Ошибка добавления",$"Аудиозапись не выбрана"));
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
                    string[]? ids = new[] {_audioModel.GetAudioIdFormatWithAccessKey()};

                    VkApiManager.AddAudioToPlaylist(item.OwnerId, item.Id, ids);

                    //Notify.NotifyManager.Instance.PopMessage(
                    //    new NotifyData("Успешно добавлено",$"Аудиозапись {_audioModel.Title} добавлена в альбом {item.Title}"));
                }
                catch (Exception ex)
                {
                    //Notify.NotifyManager.Instance.PopMessage(
                    //    new NotifyData("Ошибка добавления",$"Аудиозапись {_audioModel.Title} не добавлена в альбом {item.Title}"));
                }
            }

            CloseViewEvent?.Invoke();
        }

        protected override void LoadData()
        {
            if (_authorizationService.CurrentAccount?.UserId != null)
            {
                VkCollection<AudioPlaylist>? res = VkApiManager.GetAudioPlaylists((long) _authorizationService.CurrentAccount.UserId, 200, (uint) Offset);
                if (res != null)
                {
                    DataCollection.AddRange(res.Where(x => x.Original == null));
                    DataCollection.StartLoadImagesAsync();
                }
            }
        }

        public event ICloseView.CloseViewDelegate? CloseViewEvent;
        public IReactiveCommand CloseCommand { get; set; }
    }
}