using System.Threading.Tasks;
using ReactiveUI;
using VkNet.Model;
using VkProvider.Module;

namespace VkPlayer.Domain
{
    /// <summary>
    /// Текст композиции
    /// </summary>
    public class LyricsViewModel : ReactiveObject
    {
        public LyricsViewModel(long? lyricsId)
        {
            _id = lyricsId;
        }

        /// <summary>
        /// Начать загрузку
        /// </summary>
        public void StartLoad()
        {
            Task.Run(() =>
            {
                Lyrics? res = VkApiManager.GetLyrics((long) _id);
                Text = res.Text;
            });
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        private long? _id = 0;

        /// <summary>
        /// Текст
        /// </summary>
        public string Text {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        /// <summary>
        /// Видимость
        /// </summary>
        public bool IsVisible {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        private string _text;
        private bool _isVisible;
    }
}