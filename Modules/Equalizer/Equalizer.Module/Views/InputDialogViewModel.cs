using System.Windows.Input;
using Avalonia.Controls;
using Common.Core.Views;
using Common.Core.Views.Interfaces;
using Prism.Commands;

namespace Equalizer.Module.Views
{
    public class InputDialogViewModel : ViewModelBase, ICloseView
    {
        public InputDialogViewModel()
        {
            OkCommand = new DelegateCommand<object>(OnOk);
            CancelCommand = new DelegateCommand<object>(OnCancel);
        }

        /// <summary>
        /// Ок
        /// </summary>
        /// <param name="inputDialog"></param>
        private void OnOk(object inputDialog)
        {
            Success = true;
            OnClose(inputDialog);
        }

        /// <summary>
        /// Отмена
        /// </summary>
        /// <param name="inputDialog"></param>
        private void OnCancel(object inputDialog)
        {
            Success = false;
            OnClose(inputDialog);
        }

        /// <summary>
        /// Закрыть окно
        /// </summary>
        /// <param name="inputDialog"></param>
        private void OnClose(object inputDialog)
        {
            if (inputDialog is Window dialog)
            {
                dialog.Close(Success);
            }

            CloseViewEvent?.Invoke();
        }

        /// <summary>
        /// Сообщение
        /// </summary>
        
        public string Message { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        
        public string InputText { get; set; }

        /// <summary>
        /// Флаг
        /// </summary>
        public bool Success { get; set; }

        public event ICloseView.CloseViewDelegate? CloseViewEvent;

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }
    }
}