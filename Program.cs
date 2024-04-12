using Avalonia;
using System;
using Avalonia.Media;
using Avalonia.ReactiveUI;

namespace IoTLib_Test
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            FontManagerOptions options = new();
            if (OperatingSystem.IsLinux())
            {
                options.DefaultFamilyName = "Liberation Sans";
            }
            // No need to set default for Windows
            return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .With(options)
            .UseReactiveUI();
        }
    }
}

//TODO: Readme bearbeiten
//TODO: AppName ändern?
//TODO: UART