using System.Collections.Generic;

namespace Equalizer.Module.Domain
{
    public class EqualizerPresset
    {
        public  string Title { get; set; }
        public bool IsDefault { get; set; }
        public  List<Equalizer> Equalizers { get; set; }
    }
}
