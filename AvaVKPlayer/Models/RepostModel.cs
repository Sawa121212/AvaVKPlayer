using AvaVKPlayer.Models.Interfaces;
using VkNet.Model;

namespace AvaVKPlayer.Models
{
    public class RepostModel : IVkModelBase
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public IImageBase Image { get; set; }

        public RepostModel()
        {
            Image = new ImageModel();
        }

        public RepostModel(User user) : this()
        {
            Id = user.Id;
            Title = $"{user.FirstName} {user.LastName}";

            if (user.Photo50 != null)
                Image.ImageUrl = user.Photo50.ToString();
        }

        public RepostModel(Conversation conversation) : this()
        {
            Id = conversation.Peer.Id;

            if (conversation.ChatSettings is null)
                return;

            Title = conversation.ChatSettings.Title;
            Image.ImageUrl = conversation.ChatSettings.Photo?.Photo50?.ToString();
        }

        public RepostModel(Conversation conversation, User user) : this(conversation)
        {
            Title = $"{user.FirstName} {user.LastName}";
            Image.ImageUrl = user.Photo50.ToString();
        }

        public RepostModel(Conversation conversation, Group group) : this(conversation)
        {
            Title = $"{group.Name}";
            Image.ImageUrl = group.Photo50?.ToString();
        }
    }
}