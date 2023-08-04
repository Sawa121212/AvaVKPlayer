using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;

namespace Common.Core.ToDo
{
    public static class GlobalVars
    {
        private static string? _homedirectory;
        private static OSPlatform? _currentPlatform;

        static GlobalVars()
        {
            DefaultMusicImage = Utils.LoadImageFromAssets("MusicIcon.jpg");
            DefaultAlbumImage = Utils.LoadImageFromAssets("AlbumIcon.png");
        }

        public static string AppName => "AvaVKPlayer";

        public static string? HomeDirectory => _homedirectory ??= Utils.GetHomeDirectory();

        public static Bitmap? DefaultMusicImage { get; set; }
        public static Bitmap? DefaultAlbumImage { get; set; }

        public static OSPlatform? CurrentPlatform => _currentPlatform ??= Utils.CheckPlatForm();
    }
}