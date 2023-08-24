using System;
using System.Reactive.Linq;
using Avalonia.Layout;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkPlayer.Module.ViewModels.Base;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels.Audios
{
    public class AudioSearchViewModel : AudioViewModelBase
    {
        public AudioSearchViewModel() : base()
        {
            IsLoading = false;

            StartSearchObservable(new TimeSpan(0, 0, 1));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioRemoveIsVisible = false;
        }

        public sealed override void StartSearchObservable(TimeSpan timeSpan)
        {
            this.WhenAnyValue(vm => vm.SearchText).Throttle(timeSpan).Subscribe(text =>
            {
                if (text is not null && text.Length > 0)
                {
                    DataCollection?.Clear();
                    ResponseCount = 0;
                    Offset = 0;
                    StartLoad();
                }
            });
        }

        /// <inheritdoc />
        public override void OnSelected(AudioModel item)
        {
        }

        protected override void LoadData()
        {
            VkCollection<Audio>? res = VkApiManager.SearchAudio(new AudioSearchParams
            {
                Query = SearchText,
                Offset = Offset,
                Count = 300
            });

            if (res == null)
                return;

            DataCollection.AddRange(res);
            ResponseCount = res.Count;

            DataCollection.StartLoadImagesAsync();
            Offset += res.Count;
        }
    }
}