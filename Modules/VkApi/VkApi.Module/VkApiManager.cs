using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;

namespace VkProvider.Module
{
    public static class VkApiManager
    {
        private static VkApi? _vkApi;

        public static VkApi VkApi
        {
            get
            {
                if (_vkApi is not null)
                    return _vkApi;

                _vkApi = new VkApi();
                _vkApi.Authorize(new ApiAuthParams
                {
                    AccessToken = "4b0168fd4b0168fd4b0168fd8f4b676c6744b014b0168fd1093d8fdf1e3c0017422a04c"
                });

                return _vkApi;
            }
            set
            {
                _vkApi = value;
                Console.WriteLine("ApiChanged");
                // ToDo Events.VkaPiChangedCall();
            }
        }

        public static async Task<AccountSaveProfileInfoParams> GetProfileInfoAsync()
        {
            return await VkApi.Account.GetProfileInfoAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Lyrics GetLyrics(long id)
        {
            return VkApi.Audio.GetLyrics(id);
        }

        public static async Task<ReadOnlyCollection<User>> GetUsersAsync(long[] userId, ProfileFields photo50)
        {
            return await VkApi.Users.GetAsync(userId, ProfileFields.Photo50);
        }

        public static IEnumerable<Audio>? GetAudioById(IEnumerable<string> audioIds)
        {
            return VkApi.Audio.GetById(audioIds);
        }

        public static async Task<IEnumerable<Audio>> GetAudioByIdAsync(IEnumerable<string> audioIds)
        {
            return await VkApi.Audio.GetByIdAsync(audioIds);
        }

        public static async Task<long> AddAudioAsync(long audioId, long audioOwnerId, string accessKey)
        {
            return await VkApi.Audio.AddAsync(audioId, audioOwnerId, accessKey);
        }

        public static async Task<bool> DeleteAudioAsync(long audioId, long audioOwnerId)
        {
            return await VkApi.Audio.DeleteAsync(audioId, audioOwnerId);
        }

        public static async Task<VkCollection<Audio>?> GetAudioAsync(AudioGetParams audioGetParams)
        {
            return await VkApi.Audio.GetAsync(new AudioGetParams()
            {
                OwnerId = audioGetParams.OwnerId,
                PlaylistId = audioGetParams.PlaylistId,
                Count = 6000
            });
        }

        public static bool EditAudioPlaylist(long albumOwnerId, int albumId, string albumTitle, string o,
            IEnumerable<string> audios)
        {
            return VkApi.Audio.EditPlaylist(albumOwnerId, albumId, albumTitle, o, audios);
        }

        public static void AddAudioToPlaylist(long itemOwnerId, long itemId, IEnumerable<string>? ids)
        {
            VkApi.Audio.AddToPlaylist(itemOwnerId, itemId, ids);
        }

        public static VkCollection<AudioPlaylist> GetAudioPlaylists(long userId, uint? i, uint offset)
        {
            return VkApi.Audio.GetPlaylists(userId, i, offset);
        }

        public static VkCollection<Audio> GetAudio(AudioGetParams audioGetParams)
        {
            return VkApi.Audio.Get(new AudioGetParams
            {
                Offset = audioGetParams.Offset,
                Count = audioGetParams.Count,
            });
        }

        public static VkCollection<Audio> SearchAudio(AudioSearchParams audioSearchParams)
        {
            return VkApi.Audio.Search(new AudioSearchParams
            {
                Query = audioSearchParams.Query,
                Offset = audioSearchParams.Offset,
                Count = audioSearchParams.Count
            });
        }

        public static VkCollection<Audio> GetAudioRecommendations(long? userId, uint? count, uint offset)
        {
            return VkApi.Audio.GetRecommendations(null, userId, count, offset);
        }

        public static GetConversationsResult GetMessagesConversations(GetConversationsParams getConversationsParams)
        {
            return VkApi.Messages.GetConversations(new GetConversationsParams()
            {
                Extended = getConversationsParams.Extended,
                Count = getConversationsParams.Count,
                Offset = getConversationsParams.Offset
            });
        }

        public static VkCollection<User> GetFriends(FriendsGetParams friendsGetParams)
        {
            return VkApi.Friends.Get(new FriendsGetParams()
            {
                Fields = friendsGetParams.Fields,
                Order = friendsGetParams.Order,
            });
        }

        public static void SendMessages(MessagesSendParams messagesSendParams)
        {
            VkApi.Messages.Send(new MessagesSendParams()
            {
                PeerId = messagesSendParams.PeerId,
                RandomId = messagesSendParams.RandomId,
                Attachments = messagesSendParams.Attachments
            });
        }
    }
}