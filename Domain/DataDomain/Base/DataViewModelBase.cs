using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Common.Core.ToDo;
using Common.Core.Views;
using ReactiveUI;
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