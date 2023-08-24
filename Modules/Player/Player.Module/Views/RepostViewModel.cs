using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Common.Core.ToDo;
using Common.Core.Views.Interfaces;
using Notification.Module.Services;
using ReactiveUI;
using VkNet.Enums.Filters;
using VkNet.Enums.StringEnums;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.Base;
using VkPlayer.Domain.ETC;
using VkProvider.Module;

namespace VkPlayer.Module.Views
{
    public class RepostViewModel : DataViewModelBase<RepostModel>, ICloseView
    {
        public RepostViewModel()
        {
            CloseCommand = ReactiveCommand.Create(() => CloseViewEvent?.Invoke());

            this.WhenAnyValue(vm => vm.RepostToType)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    DataCollection?.Clear();
                    Offset = 0;
                    StartLoad();
                });
            StartScrollChangedObservable(StartLoad, Avalonia.Layout.Orientation.Vertical);
        }

        public RepostViewModel(RepostToType repostToType) : this()
        {
            RepostToType = repostToType;
        }

        public RepostViewModel(RepostToType repostToType, AudioModel audioModel) : this(repostToType)
        {
            if (audioModel != null)
            {
                AudioModel = audioModel;
            }
        }


        /// <inheritdoc />
        public override void OnSelected(RepostModel item)
        {
        }

        /// <inheritdoc />
        protected override void LoadData()
        {
            if (RepostToType == RepostToType.Friend)
            {
                LoadAllFriends();
                StopScrollChandegObserVable();
            }
            else
            {
                LoadConversation();
            }

            DataCollection.StartLoadImagesAsync();
        }

        /// <summary>
        /// Загрузить беседу
        /// </summary>
        private void LoadConversation()
        {
            GetConversationsResult? data = VkApiManager.GetMessagesConversations(new GetConversationsParams()
            {
                Extended = true,
                Count = 200,
                Offset = (ulong) (DataCollection?.Count ?? 0),
            });


            foreach (ConversationAndLastMessage? item in data.Items)
            {
                RepostModel repostModel = null;
                Conversation? conversation = item.Conversation;

                switch (conversation.Peer.Type)
                {
                    case ConversationPeerType.Chat:
                        repostModel = new RepostModel(conversation);
                        break;
                    case ConversationPeerType.User:
                    {
                        foreach (User? profile in data.Profiles)
                        {
                            if (profile.Id != conversation.Peer.Id)
                            {
                                continue;
                            }

                            repostModel = new RepostModel(conversation, profile);
                            break;
                        }

                        break;
                    }
                    case ConversationPeerType.Group:
                    {
                        foreach (Group? group in data.Groups)
                        {
                            if (@group.Id != -conversation.Peer.Id)
                            {
                                continue;
                            }

                            repostModel = new RepostModel(conversation, @group);
                            break;
                        }

                        break;
                    }
                    case ConversationPeerType.Email:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DataCollection?.Add(repostModel);
            }
        }

        /// <summary>
        /// Загрузить всех друзей
        /// </summary>
        private void LoadAllFriends()
        {
            VkCollection<User>? friends = VkApiManager.GetFriends(new FriendsGetParams()
            {
                Fields = ProfileFields.Photo50,
                Order = FriendsOrder.Hints,
            });

            if (friends == null)
                return;

            foreach (User? item in friends)
                DataCollection?.Add(new RepostModel(item));

            DataCollection.StartLoadImages();
        }

        /// <inheritdoc />
        public override void OnSelectedItem(object sender, PointerPressedEventArgs args)
        {
            RepostModel? item = args?.GetContent<RepostModel>();

            if (item != null && AudioModel != null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        VkApiManager.SendMessages(new MessagesSendParams()
                        {
                            PeerId = item.Id,
                            RandomId = Utils.Random.Next(),
                            Attachments = VkApiManager.GetAudioById(new string[]
                                {AudioModel.GetAudioIdFormatWithAccessKey()}),
                        });

                        _notificationService.Show("Успешно отправлено",
                            $"Аудиозапись отправлена: {item.Title}", NotificationType.Information);
                    }
                    catch (Exception exp)
                    {
                        _notificationService.Show("Ошибка отправки",
                            $"Возникла проблема при отправке сообщения. \n{exp.Message}", NotificationType.Information);
                    }
                    finally
                    {
                        CloseViewEvent?.Invoke();
                    }
                });
            }
        }

        private AudioModel? AudioModel { get; set; }

        private RepostToType[] RepostTypeItems { get; set; } =
        {
            RepostToType.Friend,
            RepostToType.Dialog,
        };

        public RepostToType RepostToType
        {
            get => _repostToType;
            set => this.RaiseAndSetIfChanged(ref _repostToType, value);
        }

        public string Info
        {
            get => _info;
            set => this.RaiseAndSetIfChanged(ref _info, value);
        }

        public ICommand CloseCommand { get; }

        protected readonly INotificationService _notificationService;
        public event ICloseView.CloseViewDelegate CloseViewEvent;

        private RepostToType _repostToType;
        private string _info;
    }
}