/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Bruegel                              *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;

namespace FusDotnetDemo;

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

//TODO: json files mit standardconfig
//TODO: standard config für verschiedene boards einfügen
//TODO: Readme -> standard values in json
//TODO: Readme -> wo findet man benötigte pins?
//TODO: Readme: neue boardconfig eintragen: -> cat /sys/bdinfo/platform