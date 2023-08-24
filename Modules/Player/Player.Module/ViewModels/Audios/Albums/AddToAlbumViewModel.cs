using System;
using System.Linq;
using System.Windows.Input;
using Authorization.Module.Services;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Common.Core.Views.Interfaces;
using Notification.Module.Services;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels.Audios.Albums
{
    public class AddToAlbumViewModel : AlbumsViewModel, ICloseView
    {
        private AudioModel _audioModel;

        public AddToAlbumViewModel(
            AudioModel audioModel,
            IAuthorizationService authorizationService,
            INotificationService notificationService)
            : base(authorizationService)
        {
            _authorizationService = authorizationService;
            _notificationService = notificationService;

            if (audioModel is null)
            {
                //Notify.NotifyManager.Instance.PopMessage(
                //    new NotifyData("Ошибка добавления",$"Аудиозапись не выбрана"));
                throw new ArgumentNullException(nameof(audioModel));
            }
            else
            {
                _audioModel = audioModel;
                CloseCommand = ReactiveCommand.Create(() => CloseViewEvent?.Invoke());
            }
        }

        /// <inheritdoc />
        public override void OnSelectedItem(object sender, PointerPressedEventArgs args)
        {
            AudioAlbumModel? item = args?.GetContent<AudioAlbumModel>();
            if (item != null)
            {
                try
                {
                    string[]? ids = {_audioModel.GetAudioIdFormatWithAccessKey()};

                    VkApiManager.AddAudioToPlaylist(item.OwnerId, item.Id, ids);

                    _notificationService.Show("Успешно добавлено",
                        $"Аудиозапись {_audioModel.Title} добавлена в альбом {item.Title}",
                        NotificationType.Information);
                }
                catch (Exception ex)
                {
                    _notificationService.Show("Ошибка добавления",
                        $"Аудиозапись {_audioModel.Title} не добавлена в альбом {item.Title}.\n{ex.Message}",
                        NotificationType.Error);
                }
            }

            CloseViewEvent?.Invoke();
        }

        /// <inheritdoc />
        protected override void LoadData()
        {
            if (_authorizationService.CurrentAccount?.UserId == null)
            {
                return;
            }

            VkCollection<AudioPlaylist>? res =
                VkApiManager.GetAudioPlaylists((long) _authorizationService.CurrentAccount.UserId, 200,
                    (uint) Offset);
            if (res == null)
            {
                return;
            }

            DataCollection.AddRange(res.Where(x => x.Original == null));
            DataCollection.StartLoadImagesAsync();
        }

        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;

        public ICommand CloseCommand { get; }
        public event ICloseView.CloseViewDelegate? CloseViewEvent;
    }
}