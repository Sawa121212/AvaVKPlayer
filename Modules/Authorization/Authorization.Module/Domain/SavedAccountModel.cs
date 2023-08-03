using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Core.ToDo;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkProvider.Module;

namespace Authorization.Module.Domain
{
    public class SavedAccountModel
    {
        public string? Name { get; set; }
        public long? UserId { get; set; }
        public string? Token { get; set; }

        public ImageModel Image { get; set; }

        public bool Default { get; set; } = false;


        public async void LoadAvatar()
        {
            try
            {
                ReadOnlyCollection<User> profileInfoAwaiter =
                    await VkApiManager.GetUsersAsync(new[] {(long) UserId}, ProfileFields.Photo50);
                User res = profileInfoAwaiter?.First();

                if (res == null)
                    return;

                Image ??= new ImageModel();
                Image.ImageUrl = res.Photo50.AbsoluteUri;
                Image.LoadBitmapAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}