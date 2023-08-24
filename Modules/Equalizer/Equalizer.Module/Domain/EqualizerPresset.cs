using System.Collections.Generic;
using ReactiveUI;

namespace Equalizer.Module.Domain
{
    /// <summary>
    /// Предустановки эквалайзеров
    /// </summary>
    public class EqualizerPresset : ReactiveObject
    {
        public EqualizerPresset()
        {
            Equalizers = new List<Equalizer>();
        }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        /// <summary>
        /// Эквалайзеры
        /// </summary>
        public List<Equalizer> Equalizers
        {
            get => _equalizers;
            set => this.RaiseAndSetIfChanged(ref _equalizers, value);
        }

        /// <summary>
        /// по умолчанию
        /// </summary>
        public bool IsDefault
        {
            get => _isDefault;
            set => this.RaiseAndSetIfChanged(ref _isDefault, value);
        }

        private string _title;
        private List<Equalizer> _equalizers;
        private bool _isDefault;
    }
}