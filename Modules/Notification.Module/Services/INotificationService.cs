using System;
using Avalonia;
using Avalonia.Controls.Notifications;

namespace Notification.Module.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Таймер отображения уведомления
        /// </summary>
        int NotificationTimeout { get; set; }

        /// <summary>Установить главное окно для сообщений</summary>
        /// <param name="window">Родительское окно</param>
        void SetHostWindow(IAvaloniaObject window);

        /// <summary>Показать уведомление</summary>
        /// <param name="title">Заголовок</param>
        /// <param name="message">Текст</param>
        /// <param name="notificationType">Тип уведомления</param>
        /// <param name="onClick">Выполнить действие по клику по уведомлению</param>
        void Show(string title, string message, NotificationType notificationType, Action? onClick = null);
    }
}