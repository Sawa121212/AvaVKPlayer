using System;
using VkNet.Enums.Filters;

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
                var profileInfoAwaiter = await GlobalVars.VkApi.Users
                    .GetAsync(new[] {(long) UserId}, ProfileFields.Photo50);

                var res = profileInfoAwaiter[0];
                if (res != null)
                {
                    if (Image is null)
                        Image = new ImageModel();
                    Image.ImageUrl = res.Photo50.AbsoluteUri;
                    Image.LoadBitmapAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex");
            }
        }
    }
}