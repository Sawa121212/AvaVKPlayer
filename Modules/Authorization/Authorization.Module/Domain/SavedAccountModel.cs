using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkProvider.Module;

namespace Authorization.Module.Domain
{
    /// <summary>
    /// Сохраненный аккаунт
    /// </summary>
    public class SavedAccountModel : ReactiveObject
    {
        private ImageModel _image;
        private long? _userId;
        private string? _name;
        private string? _token;
        private string _status;

        /// <summary>
        /// Идентификатор
        /// </summary>
        public long? UserId
        {
            get => _userId;
            set => this.RaiseAndSetIfChanged(ref _userId, value);
        }

        /// <summary>
        /// Имя
        /// </summary>
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>
        /// Аватар
        /// </summary>
        public ImageModel Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        /// <summary>
        /// Токен
        /// </summary>
        public string? Token
        {
            get => _token;
            set => this.RaiseAndSetIfChanged(ref _token, value);
        }

        /// <summary>
        /// Флаг, указывающий что этот аккаунт выбран для входа по умолчанию
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// Статус
        /// </summary>
        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        /// <summary>
        /// Обновить информацию об аккаунте
        /// </summary>
        public async Task UpdateInformation()
        {
            if (UserId == null)
            {
                return;
            }

            try
            {
                ReadOnlyCollection<User> profileInfoAwaiter =
                    await VkApiManager.GetUsersAsync(new[] {(long) UserId}, ProfileFields.Photo50);
                User res = profileInfoAwaiter?.First();

                if (res == null)
                    return;

                Image ??= new ImageModel();
                Image.ImageUrl = res.Photo50.AbsoluteUri;
                await Image.LoadBitmapAsync();

                AccountSaveProfileInfoParams profile = await VkApiManager.GetProfileInfoAsync();
                UserId = VkApiManager.VkApi.UserId;
                Name = $"{profile.FirstName} {profile.LastName}";
                Token = VkApiManager.VkApi.Token;
                Status = profile.Status;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}