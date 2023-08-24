using Common.Core.Views;
using ReactiveUI;

namespace VkPlayer.Domain.Base
{
    public abstract class DataViewModelBase : ViewModelBase
    {
        private bool _isError;
        private bool _isLoading;

        public bool IsError
        {
            get => _isError;
            set => this.RaiseAndSetIfChanged(ref _isError, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }
    }
}