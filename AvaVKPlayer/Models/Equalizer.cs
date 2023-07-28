using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaVKPlayer.Models
{
    public class Equalizer : ReactiveObject
    {
        public string Title
        {
            get => Hz + " гц";
        }

        public int Hz { get; set; }

        [Reactive] public int Value { get; set; }

        public Equalizer(int hz = 0)
        {
            this.Hz = hz;
        }
    }
}