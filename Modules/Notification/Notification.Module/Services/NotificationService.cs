using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using ReactiveUI;

namespace Notification.Module.Services
{
    /// <inheritdoc cref="INotificationService"/>
    public class NotificationService : ReactiveObject, INotificationService
    {
        private int _notificationTimeout = 5;
        private WindowNotificationManager? _notificationManager;

        /// <inheritdoc/>
        public void Show(string title, string message, NotificationType notificationType, Action? onClick = null)
        {
            if (_notificationManager is { } window)
            {
                Avalonia.Controls.Notifications.Notification notification = new(
                    title, message, notificationType, TimeSpan.FromSeconds(_notificationTimeout),
                    onClick);
                //ToDO: Fix this - window.Show(notification);
            }
            else
            {
                throw new Exception("Host window not set");
            }
        }

        /// <inheritdoc/>
        public void SetHostWindow(IAvaloniaObject hostWindow)
        {
            if (hostWindow is not Window window)
            {
                return;
            }

            WindowNotificationManager? notificationManager = new(window)
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

        private WindowNotificationManager? NotificationManager
        {
            get => _notificationManager;
            set => this.RaiseAndSetIfChanged(ref _notificationManager, value);
        }
    }
}