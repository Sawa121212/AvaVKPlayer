using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;

namespace AvaVKPlayer.Models.Interfaces
{
    public interface IImageBase
    {
        public string ImageUrl { get; set; }
        public bool ImageIsloaded { get; set; }

        [JsonIgnore] public Bitmap? Bitmap { get; set; }

        public Task<Stream?>? LoadImageStreamAsync();

        public void LoadBitmapAsync()
        {
        }
    }
}