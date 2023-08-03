using Avalonia.Layout;
using Player.Domain.ETC;
using Player.Module.ViewModels.Base;
using VkNet.Model;
using VkNet.Utils;
using VkProvider.Module;

namespace Player.Module.ViewModels.Audios
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
            VkCollection<Audio>? res =
                VkApiManager.GetAudioRecommendations(VkApiManager.VkApi.UserId, 500, offset: (uint) Offset);
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