using System;
using System.Collections.Generic;
using AvaVKPlayer.Models;
using ManagedBass;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaVKPlayer.ViewModels
{
    public class EqualizerViewModel:ReactiveObject
    {

        private int[] _channels;
    
        private List<IDisposable?> _disposibles;
    
        [Reactive]
        public  bool EqualizerManagerIsVisible { get; set; }

        [Reactive]
        public string EqualizerTitle { get; set; }
    
        [Reactive]
        public bool IsUseEqualizer { get; set; }

        [Reactive]
        public bool IsEnabled { get; set; }

        [Reactive]
        public  IReactiveCommand ResetCommand { get; set; }

        public  IReactiveCommand OpenPresetManager { get; set; }
    
        [Reactive]
        public List<Equalizer> Equalizers { get; set; }
    
        [Reactive]
        public EqualizerPresetMenagerViewModel PresetMenagerViewModel { get; set; }
    
        public EqualizerViewModel()
        {
       
            PresetMenagerViewModel = new EqualizerPresetMenagerViewModel();
        
            PresetMenagerViewModel.CloseViewEvent+=PresetMenagerViewModelOnCloseViewEvent;
        
            OpenPresetManager = ReactiveCommand.Create(() =>
            {
                EqualizerManagerIsVisible = true;
            });
        
            _disposibles = new List<IDisposable?>();
        
        
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
    
        
        
            _channels = new int[8];
        
        
            PresetMenagerViewModel
                .SavedEqualizerData
                .WhenAnyValue(x => x.SelectedPresset).Subscribe((x) =>
                {
                    if (x > -1)
                    {
                        EqualizerPresset? preset = PresetMenagerViewModel.SavedEqualizerData.EqualizerPressets[x];

                        IsEnabled = !preset.IsDefault;
                    
                        EqualizerTitle = preset.Title;

                        for (int i = 0; i < _disposibles?.Count; )
                        {
                            _disposibles[i].Dispose();
                            _disposibles.RemoveAt(i);
                        }
                
                        Equalizers = preset.Equalizers;
                
                        for (int i = 0; i < Equalizers.Count; i++)
                        {
                            IDisposable? disposible = Equalizers[i].WhenAnyValue(x => x.Value)
                                .Subscribe((val)=>
                                {
                                    if(IsUseEqualizer) UpdateFx();
                                });
                            _disposibles.Add(disposible);
                        }

                    }
                });
        }

        private void PresetMenagerViewModelOnCloseViewEvent()
        {
            EqualizerManagerIsVisible = false;
        }


        public void DisableEqualizer()
        {
            for(int i=0; i < Equalizers?.Count; i++)
            {
                ManagedBass.DirectX8.DXParamEQParameters dXParamEqParameters = new ManagedBass.DirectX8.DXParamEQParameters()
                {
                    fBandwidth = 12,
                    fCenter = Equalizers[i].Hz,
                    fGain = 0,
                };
                Bass.FXSetParameters(_channels[i], dXParamEqParameters);
            }
        }
        public void ResetEqualizer()
        {
            for (int i = 0; i < Equalizers?.Count; i++)
            {
                Equalizers[i].Value = 0;
            }
        }
        public void UpdateEqualizer()
        {
            for(int i=0; i<Equalizers?.Count; i++)
            {
                _channels[i] = Bass.ChannelSetFX(PlayerControlViewModel.Player.GetStreamHandler(), EffectType.DXParamEQ, 0);
            }
        }

        public void UpdateFx()
        {
            for(int i=0; i < Equalizers?.Count; i++)
            {
                ManagedBass.DirectX8.DXParamEQParameters dXParamEqParameters = new ManagedBass.DirectX8.DXParamEQParameters()
                {
                    fBandwidth = 12,
                    fCenter = Equalizers[i].Hz,
                    fGain = Equalizers[i].Value,
                };
                Bass.FXSetParameters(_channels[i], dXParamEqParameters);
            }
            PresetMenagerViewModel.SavePressets();
        }

   
    }
}