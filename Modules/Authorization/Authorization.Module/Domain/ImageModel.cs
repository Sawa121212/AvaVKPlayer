using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Authorization.Module.Domain.Interfaces;
using Avalonia.Media.Imaging;
using Common.Core.ToDo;
using Newtonsoft.Json;
using ReactiveUI;

namespace Authorization.Module.Domain
{
    public class ImageModel : ReactiveObject, IImageBase
    {
        ~ImageModel()
        {
            if (Bitmap != null && ImageIsloaded)
            {
                Bitmap.Dispose();
            }
        }

        /// <inheritdoc />
        public async Task<Stream?> LoadImageStreamAsync()
        {
            return await Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(ImageUrl))
                        return null;

                    byte[]? bytes = null;

                    bytes = await Utils.HttpClient.GetByteArrayAsync(ImageUrl);

                    return new MemoryStream(bytes);
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        /// <inheritdoc />
        public virtual async Task LoadBitmapAsync()
        {
            if (string.IsNullOrEmpty(ImageUrl) is false && ImageIsloaded is false)
            {
                try
                {
                    Semaphore.WaitOne();
                    await using (Stream? imageStream = await LoadImageStreamAsync())
                    {
                        if (imageStream is null)
                            return;

                        Bitmap = await Task.Run(() =>
                            DecodeWidth <= 0 ? new Bitmap(imageStream) : Bitmap.DecodeToWidth(imageStream, DecodeWidth)
                        );

                        ImageIsloaded = true;
                    }
                }
                finally
                {
                    Semaphore.Release();
                }
            }
        }

        public int DecodeWidth { get; set; }

        /// <inheritdoc />
        public string ImageUrl { get; set; }

        /// <inheritdoc />
        public bool ImageIsloaded { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public Bitmap? Bitmap
        {
            get => _bitmap;
            set => this.RaiseAndSetIfChanged(ref _bitmap, value);
        }

        public static Semaphore Semaphore = new(50, 50);
        private Bitmap _bitmap;
    }
}