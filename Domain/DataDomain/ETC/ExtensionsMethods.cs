using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Player.Domain.Interfaces;
using VkNet.Model;

namespace Player.Domain.ETC
{
    public static class ExtensionsMethods
    {
        public static T? GetContent<T>(this PointerPressedEventArgs eventArgs) where T : class
        {
            T? res = (eventArgs?.Source as ContentPresenter)?.Content as T;
            if (res is null)
                res = (eventArgs?.Source as TextBlock)?.DataContext as T;

            return res;
        }


        public static void AddRange(this ObservableCollection<AudioModel>? dataCollection,
            IEnumerable<Audio>? audios)
        {
            if (audios != null)
            {
                foreach (Audio? item in audios)
                    dataCollection?.Add(new AudioModel(item));
            }
        }

        public static void AddRange(this ObservableCollection<AudioAlbumModel>? dataCollection,
            IEnumerable<AudioPlaylist> audioPlayList)
        {
            if (audioPlayList != null)
            {
                foreach (AudioPlaylist? item in audioPlayList)
                    dataCollection?.Add(new AudioAlbumModel(item));
            }
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            int index = 0;
            bool isSearched = false;
            foreach (T? item in items)
            {
                if (predicate(item))
                {
                    isSearched = true;
                    break;
                }

                index++;
            }

            return isSearched ? index : -1;
        }

        public static void StartLoadImages<T>(this ObservableCollection<T>? dataCollection) where T : IVkModelBase
        {
            try
            {
                int? itemCount = dataCollection?.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    if (dataCollection[i] != null)
                        dataCollection[i].Image.LoadBitmapAsync();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void StartLoadImagesAsync<T>(this ObservableCollection<T>? dataCollection) where T : IVkModelBase
        {
            Task.Run(() => StartLoadImages(dataCollection));
        }


        public static ObservableCollection<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
                return new ObservableCollection<T>();

            ObservableCollection<T> obscollection = new(collection);
            Random rand = new();
            int itercount = collection.Count();

            for (int i = 0; i < itercount; i++)
            {
                T? element = obscollection[rand.Next(itercount)];
                obscollection.Remove(element);
                obscollection.Insert(rand.Next(itercount), element);
            }

            return obscollection;
        }

        public static string GetAudioIdFormatWithAccessKey(this Audio audioModel)
        {
            return $"{audioModel.OwnerId}_{audioModel.Id}_{audioModel.AccessKey}";
        }

        public static string GetAudioIdFormatNoAccessKey(this AudioModel audioModel)
        {
            return $"{audioModel.OwnerId}_{audioModel.Id}";
        }

        public static string GetAudioIdFormatWithAccessKey(this AudioModel audioModel)
        {
            return $"{audioModel.OwnerId}_{audioModel.Id}_{audioModel.AccessKey}";
        }

        public static string GetAudioIdFormatNoAccessKey(this Audio audioModel)
        {
            return $"{audioModel.OwnerId}_{audioModel.Id}";
        }
    }
}