﻿using System;
using System.Threading.Tasks;
using Avalonia.Input;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.ViewModels.Interfaces;
using Common.Core.ToDo;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Enums.Filters;
using VkNet.Enums.StringEnums;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels
{
    public class RepostViewModel : ViewModels.Base.DataViewModelBase<RepostModel>, ICloseView
    {

        private AudioModel? AudioModel { get; set; }
        private RepostToType[] RepostTypeItems { get; set; } = new[]
        {
            RepostToType.Friend,
            RepostToType.Dialog,
        };


        [Reactive]
        public RepostToType RepostToType { get; set; }
        [Reactive]
        public string Info { get; set; }
        public IReactiveCommand CloseCommand { get; set; }


        public RepostViewModel(RepostToType repostToType)
        {
            CloseCommand = ReactiveCommand.Create(() => CloseViewEvent?.Invoke());

            this.RepostToType = repostToType;

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

        public RepostViewModel(RepostToType repostToType,
                                AudioModel audioModel) : this(repostToType)
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
            GetConversationsResult? data = ETC.GlobalVars.VkApi.Messages.GetConversations(new GetConversationsParams()
            {
                Extended = true,
                Count = 200,
                Offset = (ulong)(DataCollection?.Count ?? 0),

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
            VkCollection<User>? friends = ETC.GlobalVars.VkApi.Friends.Get(new FriendsGetParams()
            {
                Fields = ProfileFields.Photo50,
                Order = FriendsOrder.Hints,
            });
            if (friends != null)
            {
                foreach (User? item in friends)
                    DataCollection?.Add(new RepostModel(item));

                DataCollection.StartLoadImages();

            }
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
                        GlobalVars.VkApi.Messages.Send(new MessagesSendParams()
                        {
                            PeerId = item.Id,
                            RandomId = Utils.Random.Next(),
                            Attachments = GlobalVars.VkApi.Audio.GetById(new String[] { AudioModel.GetAudioIdFormatWithAccessKey() }),
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
                    finally { CloseViewEvent?.Invoke(); }
                });
            }
        }

    }

}
