using System;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using AvaVKPlayer.Models;
using VkNet;
using VkNet.Model;

namespace AvaVKPlayer.ETC
{
    public static class GlobalVars
    {
        public delegate void Api();

        private static string? _homedirectory;
        private static OSPlatform? _currentPlatform;
        private static VkApi? _vkApi;

        static GlobalVars()
        {
            DefaultMusicImage = Utils.LoadImageFromAssets("MusicIcon.jpg");
            DefaultAlbumImage = Utils.LoadImageFromAssets("AlbumIcon.png");
        }

        public static SavedAccountModel? CurrentAccount { get; set; }
        public static string AppName => "AvaVKPlayer";
        public static string SavedAccountsFileName => "Accounts";

        public static string? HomeDirectory
        {
            get
            {
                if (_homedirectory == null)
                    _homedirectory = Utils.GetHomeDirectory();

                return _homedirectory;
            }
        }

        public static Bitmap? DefaultMusicImage { get; set; }
        public static Bitmap? DefaultAlbumImage { get; set; }

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
                Events.VkaPiChangedCall();
            }
        }

        public static OSPlatform? CurrentPlatform => _currentPlatform ?? (_currentPlatform = Utils.CheckPlatForm());
    }
}