using ReactiveUI;

namespace Equalizer.Module.Domain
{
    /// <summary>
    /// Эквалайзер
    /// </summary>
    public class Equalizer : ReactiveObject
    {
        public Equalizer(int hz = 0)
        {
            Hz = hz;
        }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title
        {
            get => Hz + " гц";
        }

        /// <summary>
        /// Гц
        /// </summary>
        public int Hz
        {
            get => _hz;
            set => this.RaiseAndSetIfChanged(ref _hz, value);
        }

        /// <summary>
        /// Значение
        /// </summary>

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private int _hz;
        private int _value;
    }
}