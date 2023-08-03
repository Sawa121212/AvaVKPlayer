using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;

namespace Notification.Module.Services
{
    /// <summary>
    /// Сервис отображения уведомления
    /// </summary>
    public class NotificationService : INotificationService
    {
        private int _notificationTimeout = 5;
        private WindowNotificationManager? _notificationManager;

        /// <inheritdoc/>
        public void Show(string title, string message,
            NotificationType notificationType, Action? onClick = null)
        {
            if (_notificationManager is { } nm)
            {
                Dispatcher.UIThread.InvokeAsync(() => nm.Show(
                    new Avalonia.Controls.Notifications.Notification(
                        title, message, notificationType, TimeSpan.FromSeconds(_notificationTimeout),
                        onClick)));
            }
        }

        /// <inheritdoc/>
        public void SetHostWindow(IAvaloniaObject hostWindow)
        {
            if (hostWindow is not Window window) return;
            var notificationManager = new WindowNotificationManager(window)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 4,
                Margin = new Thickness(0, 0, 15, 40)
            };

            _notificationManager = notificationManager;
        }

        /// <inheritdoc/>
        public int NotificationTimeout
        {
            get => _notificationTimeout;
            set => _notificationTimeout = (value < 0) ? 0 : value;
        }
    }
}