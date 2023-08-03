using System;
using System.Threading;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Logging;
using Avalonia.ReactiveUI;

namespace AvaVKPlayer
{
    class Program
    {
        private const int TimeoutSeconds = 3;

        [STAThread]
        public static void Main(string[] args)
        {
            // добавим Mutex для блокировки запуска повторного ПО
            Mutex mutex = new(false, typeof(Program).FullName);

            try
            {
                if (!mutex.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), true))
                {
                    return; // ждем TimeoutSeconds секунд
                }

                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            AppBuilder? builder = AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .With(new SkiaOptions()
                {
                    // 1 GB = 1,073,741,824 Byte
                    MaxGpuResourceSizeBytes = new long?(1073741824L)
                })
                .With(new Win32PlatformOptions
                {
                    EnableMultitouch = true,
                    AllowEglInitialization = true,
                    UseWindowsUIComposition = false,
                    OverlayPopups = true,
                    UseDeferredRendering = true
                })
                .With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    UseGpu = true,
                    OverlayPopups = true,
                    UseDeferredRendering = true
                })
                .With(new AvaloniaNativePlatformOptions()
                {
                    UseGpu = true,
                    OverlayPopups = true,
                    UseDeferredRendering = true
                })
                .UseSkia()
                .UseReactiveUI()
                .UseManagedSystemDialogs();
            ;

#if DEBUG
            builder.LogToTrace(LogEventLevel.Debug, LogArea.Property, LogArea.Layout, LogArea.Binding);
#endif
            return builder;
        }
    }
}