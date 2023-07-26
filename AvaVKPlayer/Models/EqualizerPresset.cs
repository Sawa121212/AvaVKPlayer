using System.Collections.Generic;

namespace AvaVKPlayer.Models
{
    public class EqualizerPresset
    {
        public  string Title { get; set; }
        public bool IsDefault { get; set; }
        public  List<Equalizer> Equalizers { get; set; }
    }
}
