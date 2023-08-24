using System.Threading.Tasks;
using ReactiveUI;
using VkNet.Model;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels
{
    public class LyricsViewModel : ReactiveObject
    {
        public string Text { get; set; }

        public bool IsVisible { get; set; } = false;

        private long? _id = 0;

        public LyricsViewModel(long? lyricsId)
        {
            _id = lyricsId;
        }

        public void StartLoad()
        {
            Task.Run(() =>
            {
                Lyrics? res = VkApiManager.GetLyrics((long) _id);
                Text = res.Text;
            });
        }
    }
}