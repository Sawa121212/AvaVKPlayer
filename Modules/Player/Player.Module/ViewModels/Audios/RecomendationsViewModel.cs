using Avalonia.Layout;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkPlayer.Module.ViewModels.Base;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels.Audios
{
    public sealed class RecomendationsViewModel : AudioViewModelBase
    {
        public RecomendationsViewModel()
        {
            SearchIsVisible = false;

            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            AudioListButtons.AudioRemoveIsVisible = false;
        }

        /// <inheritdoc />
        public override void OnSelected(AudioModel item)
        {
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