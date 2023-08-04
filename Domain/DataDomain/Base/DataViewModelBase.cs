using Common.Core.Views;
using ReactiveUI.Fody.Helpers;

namespace Player.Domain.Base
{
    public abstract class DataViewModelBase : ViewModelBase
    {
        // ToDo [Reactive] public ExceptionViewModel ExceptionModel { get; set; }

        [Reactive] public bool IsError { get; set; }

        [Reactive] public bool IsLoading { get; set; }
    }
}