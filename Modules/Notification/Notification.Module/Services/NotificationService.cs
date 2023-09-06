using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace Notification.Module.Services
{
    /// <inheritdoc cref="INotificationService"/>
    public class NotificationService : INotificationService
    {
        private int _notificationTimeout = 7;

        private WindowNotificationManager NotificationManager
        {
            get; 
            set;
        }

        /// <inheritdoc/>
        public void Show(string title, string message, NotificationType notificationType, Action? onClick = null)
        {
            if (NotificationManager is { } window)
            {
                window.Show(
                    new Avalonia.Controls.Notifications.Notification(
                        title,
                        message,
                        notificationType,
                        TimeSpan.FromSeconds(_notificationTimeout),
                        onClick));
            }
            else
            {
                //throw new Exception("Host window not set");
            }
        }

        /// <inheritdoc/>
        public void SetHostWindow(IAvaloniaObject hostWindow)
        {
            if (hostWindow is not Window window)
            {
                return;
            }

            WindowNotificationManager notificationManager = new(window)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 4,
                Margin = new Thickness(0, 0, 15, 40)
            };

            NotificationManager = notificationManager;
        }

        /// <inheritdoc/>
        public int NotificationTimeout
        {
            get => _notificationTimeout;
            set => _notificationTimeout = (value < 0) ? 0 : value;
        }
    }
}