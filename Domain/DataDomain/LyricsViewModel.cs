using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;
using VkProvider.Module;

namespace Player.Domain
{
    public class LyricsViewModel : ReactiveObject
    {
        [Reactive] public string Text { get; set; }

        [Reactive] public bool IsVisible { get; set; } = false;

        private long? _id = 0;

        public LyricsViewModel(long? lyricsId)
        {
            this._id = lyricsId;
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