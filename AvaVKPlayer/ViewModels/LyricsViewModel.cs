using System.Threading.Tasks;
using AvaVKPlayer.ETC;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;

namespace AvaVKPlayer.ViewModels
{
    public class LyricsViewModel:ReactiveObject
    {
        [Reactive]
        public  string Text { get; set; }

        [Reactive] 
        public bool IsVisible { get; set; } = false;
  
        private long? _id = 0;

        public LyricsViewModel(long? lyricsId)
        {
            this._id = lyricsId;
      
      
        }

        public void StartLoad()
        {
            Task.Run(() =>
            {
                Lyrics? res = GlobalVars.VkApi.Audio.GetLyrics((long)_id);
                Text = res.Text;
            });
        }
    }
}