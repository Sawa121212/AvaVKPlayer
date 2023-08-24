using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace Equalizer.Module.Domain
{
    /// <summary>
    /// Сохраненные данные эквалайзера
    /// </summary>
    public class SavedEqualizerData : ReactiveObject
    {
        public SavedEqualizerData()
        {
            EqualizerPressets = new ObservableCollection<EqualizerPresset>();
        }

        /// <summary>
        /// Добавить предустановку эквалайзера
        /// </summary>
        /// <param name="presset"></param>
        public void AddPreset(EqualizerPresset presset)
        {
            EqualizerPressets.Add(presset);
        }

        /// <summary>
        /// Удалить предустановку эквалайзера
        /// </summary>
        /// <param name="index"></param>
        public void RemovePreset(int index)
        {
            EqualizerPressets.RemoveAt(index);
        }

        /// <summary>
        /// Удалить предустановку эквалайзера
        /// </summary>
        /// <param name="presset"></param>
        public void RemovePreset(EqualizerPresset presset)
        {
            EqualizerPressets.Remove(presset);
        }


        /// <summary>
        /// Удалить выбранную предустановку эквалайзера
        /// </summary>
        public void RemoveSelectedPreset()
        {
            try
            {
                EqualizerPressets.RemoveAt(SelectedPresset);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Получить количество предустановок
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return EqualizerPressets.Count;
        }

        /// <summary>
        /// Предустановки эквалайзеров
        /// </summary>

        public ObservableCollection<EqualizerPresset> EqualizerPressets
        {
            get => _equalizerPressets;
            set => this.RaiseAndSetIfChanged(ref _equalizerPressets, value);
        }

        /// <summary>
        /// Выбранная предустановки
        /// </summary>

        public int SelectedPresset
        {
            get => _selectedPresset;
            set => this.RaiseAndSetIfChanged(ref _selectedPresset, value);
        }

        private ObservableCollection<EqualizerPresset> _equalizerPressets;
        private int _selectedPresset;
    }
}