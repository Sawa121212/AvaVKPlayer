using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;

namespace Authorization.Module.Domain.Interfaces
{
    public interface IImageBase
    {
        public string ImageUrl { get; set; }

        public bool ImageIsloaded { get; set; }

        /// <summary>
        /// Растровое изображение
        /// </summary>
        [JsonIgnore]
        public Bitmap? Bitmap { get; set; }

        public Task<Stream?>? LoadImageStreamAsync();

        public Task LoadBitmapAsync();
    }
}