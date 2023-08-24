using System;
using System.Collections.Generic;
using System.Windows.Input;
using Equalizer.Module.Domain;
using ManagedBass;
using ManagedBass.DirectX8;
using ReactiveUI;

namespace Equalizer.Module.Views
{
    public class EqualizerControlViewModel : ReactiveObject
    {
        public EqualizerControlViewModel()
        {
            _channels = new int[8];
            _disposibles = new List<IDisposable?>();

            PresetMenagerViewModel = new EqualizerPresetsManagerViewModel();
            PresetMenagerViewModel.CloseViewEvent += PresetManagerViewModelOnCloseViewEvent;

            PresetMenagerViewModel
                .SavedEqualizerData
                .WhenAnyValue(x => x.SelectedPresset).Subscribe((x) =>
                {
                    if (x > -1)
                    {
                        EqualizerPresset? preset = PresetMenagerViewModel.SavedEqualizerData.EqualizerPressets[x];

                        IsEnabled = !preset.IsDefault;

                        EqualizerTitle = preset.Title;

                        for (int i = 0; i < _disposibles?.Count;)
                        {
                            _disposibles[i].Dispose();
                            _disposibles.RemoveAt(i);
                        }

                        Equalizers = preset.Equalizers;

                        for (int i = 0; i < Equalizers.Count; i++)
                        {
                            IDisposable? disposible = Equalizers[i].WhenAnyValue(x => x.Value)
                                .Subscribe((val) =>
                                {
                                    if (IsUseEqualizer) UpdateFx();
                                });
                            _disposibles.Add(disposible);
                        }
                    }
                });

            this.WhenAnyValue(x => x.IsUseEqualizer).Subscribe((val) =>
            {
                if (val is false)
                {
                    DisableEqualizer();
                }
                else
                {
                    UpdateFx();
                }
            });

            OpenPresetManager = ReactiveCommand.Create(() => { EqualizerManagerIsVisible = true; });
        }


        /// <summary>
        /// Выключить эквалайзер
        /// </summary>
        private void DisableEqualizer()
        {
            for (int i = 0; i < Equalizers?.Count; i++)
            {
                DXParamEQParameters dXParamEqParameters = new()
                {
                    fBandwidth = 12,
                    fCenter = Equalizers[i].Hz,
                    fGain = 0,
                };
                Bass.FXSetParameters(_channels[i], dXParamEqParameters);
            }
        }

        /// <summary>
        /// Сбросить настройки эквалайзера
        /// </summary>
        public void ResetEqualizer()
        {
            for (int i = 0; i < Equalizers?.Count; i++)
            {
                Equalizers[i].Value = 0;
            }
        }

        /// <summary>
        /// Обновить эквалайзер
        /// </summary>
        public void UpdateEqualizer()
        {
            for (int i = 0; i < Equalizers?.Count; i++)
            {
                //_channels[i] = Bass.ChannelSetFX(PlayerControlViewModel.Player.GetStreamHandler(), EffectType.DXParamEQ,0);
            }
        }

        /// <summary>
        /// Обновить эффекты
        /// </summary>
        public void UpdateFx()
        {
            for (int i = 0; i < Equalizers?.Count; i++)
            {
                DXParamEQParameters dXParamEqParameters =
                    new()
                    {
                        fBandwidth = 12,
                        fCenter = Equalizers[i].Hz,
                        fGain = Equalizers[i].Value,
                    };
                Bass.FXSetParameters(_channels[i], dXParamEqParameters);
            }

            PresetMenagerViewModel.SavePressets();
        }

        private void PresetManagerViewModelOnCloseViewEvent()
        {
            EqualizerManagerIsVisible = false;
        }

        private int[] _channels;

        private List<IDisposable?> _disposibles;

        public bool EqualizerManagerIsVisible { get; set; }

        public string EqualizerTitle { get; set; }

        public bool IsUseEqualizer { get; set; }

        public bool IsEnabled { get; set; }

        public List<Domain.Equalizer> Equalizers { get; set; }

        public EqualizerPresetsManagerViewModel PresetMenagerViewModel { get; set; }

        public ICommand ResetCommand { get; set; }

        public ICommand OpenPresetManager { get; }
    }
}