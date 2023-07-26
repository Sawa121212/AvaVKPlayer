﻿using Avalonia.Layout;
using AvaVKPlayer.ETC;
using AvaVKPlayer.ViewModels.Base;

namespace AvaVKPlayer.ViewModels.Audios
{
    public sealed class RecomendationsViewModel : AudioViewModelBase
    {
        public RecomendationsViewModel()
        {
            SearchIsVisible = false;
           
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            AudioListButtons.AudioRemoveIsVisible = false;
        }

        protected override void LoadData()
        {
            var res = GlobalVars.VkApi?.Audio.GetRecommendations(count: 500, offset: (uint)Offset);
            if (res != null)
            {
                DataCollection.AddRange(res);

                DataCollection.StartLoadImagesAsync();

                Offset += res.Count;
                ResponseCount = res.Count;
               
            }
        }
    }
}