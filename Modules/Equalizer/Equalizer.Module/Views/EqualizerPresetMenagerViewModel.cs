using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Common.Core.Views.Interfaces;
using Equalizer.Module.Domain;
using ReactiveUI;

namespace Equalizer.Module.Views
{
    public class EqualizerPresetsManagerViewModel : ReactiveObject, ICloseView
    {
        public EqualizerPresetsManagerViewModel()
        {
            LoadingPressets();

            TitleInputViewModel = new InputDialogViewModel()
            {
                Message = "Название пресета",
            };

            // ToDo
            //TitleInputViewModel.CloseViewEvent += TextInputViewModelOnCloseViewEvent;

            AddPreset = ReactiveCommand.Create(() => { TitleInputIsVisible = true; });
            RemovePreset = ReactiveCommand.Create((EqualizerPresset p) =>
            {
                SavedEqualizerData?.RemovePreset(p);
                ApplyPreset(0);
            });
            CloseCommand = ReactiveCommand.Create(() =>
            {
                SavePressets();
                CloseViewEvent?.Invoke();
            });
        }

        /// <summary>
        /// Применить прессет
        /// </summary>
        /// <param name="index"></param>
        public void ApplyPreset(int index)
        {
            SavedEqualizerData.SelectedPresset = index;
        }


        private void TextInputViewModelOnCloseViewEvent()
        {
            if (TitleInputViewModel.Success)

            {
                TitleInputViewModel.Success = false;

                EqualizerPresset? preset = new EqualizerPresset();

                preset.Title = TitleInputViewModel.InputText;

                preset.Equalizers = _hz.Select(x => new Domain.Equalizer(x)).ToList();

                SavedEqualizerData.AddPreset(preset);

                TitleInputViewModel.InputText = string.Empty;
                SavePressets();
            }

            TitleInputIsVisible = false;
        }

        /// <summary>
        /// Загрузить прессеты
        /// </summary>
        private void LoadingPressets()
        {
            try
            {
                SavedEqualizerData =
                    JsonSerializer.Deserialize<SavedEqualizerData>(File.ReadAllText(FileName));
            }
            catch (Exception ex)
            {
                SavedEqualizerData = new SavedEqualizerData();
            }
            finally
            {
                if (SavedEqualizerData?.GetCount() == 0 ||
                    SavedEqualizerData?.EqualizerPressets.Count(x => x.Title == DefaultPresetName) == 0)
                {
                    EqualizerPresset presset = new()
                    {
                        Title = DefaultPresetName,
                        Equalizers = _hz.Select(x => new Domain.Equalizer(x)).ToList(),
                        IsDefault = true
                    };
                    SavedEqualizerData.EqualizerPressets.Insert(0, presset);
                }
            }
        }

        /// <summary>
        /// Сохранить прессеты
        /// </summary>
        public void SavePressets()
        {
            File.WriteAllText(FileName, JsonSerializer.Serialize(SavedEqualizerData).Trim());
        }

        private int[] _hz = new int[] {80, 170, 310, 600, 1000, 3000, 6000, 12000};

        public const string FileName = "EqualizerPressets.json";
        public const string DefaultPresetName = "Обычный";


        public bool TitleInputIsVisible { get; set; }

        public InputDialogViewModel TitleInputViewModel { get; set; }

        public SavedEqualizerData SavedEqualizerData { get; private set; }

        public IReactiveCommand AddPreset { get; }
        public IReactiveCommand RemovePreset { get; }

        public ICommand CloseCommand { get; }
        public event ICloseView.CloseViewDelegate? CloseViewEvent;
    }
}