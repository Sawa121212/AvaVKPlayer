using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Common.Core.ToDo;
using Common.Core.Views.Interfaces;
using Player.Domain;
using Player.Domain.Base;
using Player.Domain.ETC;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Enums.Filters;
using VkNet.Enums.StringEnums;
using VkNet.Model;
using VkNet.Utils;
using VkProvider.Module;

namespace Player.Module.Views
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
            this.RepostToType = repostToType;
        }

        public RepostViewModel(RepostToType repostToType, AudioModel audioModel) : this(repostToType)
        {
            if (audioModel != null)
            {
                AudioModel = audioModel;
            }
        }

        public event ICloseView.CloseViewDelegate CloseViewEvent;

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

                if (conversation.Peer.Type == ConversationPeerType.Chat)
                    repostModel = new RepostModel(conversation);

                else if (conversation.Peer.Type == ConversationPeerType.User)
                {
                    foreach (User? profile in data.Profiles)
                    {
                        if (profile.Id == conversation.Peer.Id)
                        {
                            repostModel = new RepostModel(conversation, profile);
                            break;
                        }
                    }
                }
                else if (conversation.Peer.Type == ConversationPeerType.Group)
                {
                    foreach (Group? group in data.Groups)
                    {
                        if (group.Id == -conversation.Peer.Id)
                        {
                            repostModel = new RepostModel(conversation, group);
                            break;
                        }
                    }
                }

                DataCollection?.Add(repostModel);
            }
        }

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

        public override void SelectedItem(object sender, PointerPressedEventArgs args)
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
                        /*Notify.NotifyManager.Instance.PopMessage(
                            new Notify.NotifyData("Успешно отправлено", "Аудиозапись отправлена: " + item.Title
                            , TimeSpan.FromSeconds(2)));*/
                    }
                    catch (Exception)
                    {
                        /*Notify.NotifyManager.Instance.PopMessage(
                            new Notify.NotifyData("Ошибка отправки", "Возникла проблема при отправке сообщения",
                            TimeSpan.FromSeconds(2)));*/
                    }
                    finally
                    {
                        CloseViewEvent?.Invoke();
                    }
                });
            }
        }

        private AudioModel? AudioModel { get; set; }

        private RepostToType[] RepostTypeItems { get; set; } = new[]
        {
            RepostToType.Friend,
            RepostToType.Dialog,
        };

        [Reactive] public RepostToType RepostToType { get; set; }
        [Reactive] public string Info { get; set; }
        public IReactiveCommand CloseCommand { get; set; }
    }
}