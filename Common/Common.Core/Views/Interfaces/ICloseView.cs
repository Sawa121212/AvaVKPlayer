using System.Windows.Input;

namespace Common.Core.Views.Interfaces
{
    public interface ICloseView
    {
        public delegate void CloseViewDelegate();

        public event CloseViewDelegate CloseViewEvent;
        public ICommand CloseCommand { get; }
    }
}