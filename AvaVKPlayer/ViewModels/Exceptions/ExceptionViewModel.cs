using System;
using AvaVKPlayer.ETC;
using AvaVKPlayer.ViewModels.Base;
using ReactiveUI;

namespace AvaVKPlayer.ViewModels.Exceptions
{
    public class ExceptionViewModel : ReactiveObject
    {
        public delegate void ViewExit();

        private string _buttonMessage;

        private string _errorMessage;
        private int _gridColumn;
        private int _gridColumnSpan;
        private int _gridRow;
        private int _gridRowSpan;
        private bool _isVisible;

        public ExceptionViewModel()
        {
            CallActionCommand = ReactiveCommand.Create(() =>
            {
                ViewExitEvent?.Invoke();
                InvokeHandler.Start(new InvokeHandlerObject(Action, View));
            });
        }

        public string ButtonMessage
        {
            get => _buttonMessage;
            set => this.RaiseAndSetIfChanged(ref _buttonMessage, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        public int GridRowSpan
        {
            get => _gridRowSpan;
            set => this.RaiseAndSetIfChanged(ref _gridRowSpan, value);
        }

        public int GridColumnSpan
        {
            get => _gridColumnSpan;
            set => this.RaiseAndSetIfChanged(ref _gridColumnSpan, value);
        }

        public int GridRow
        {
            get => _gridRow;
            set => this.RaiseAndSetIfChanged(ref _gridRow, value);
        }

        public int GridColumn
        {
            get => _gridColumn;
            set => this.RaiseAndSetIfChanged(ref _gridColumn, value);
        }

        public Action Action { get; set; }
        public DataViewModelBase View { get; set; }

        public IReactiveCommand ExitCommand { get; set; }
        public IReactiveCommand CallActionCommand { get; set; }

        public static event ViewExit ViewExitEvent;
    }
}