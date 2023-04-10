﻿using Microsoft.Extensions.Logging;
using SecureConnection.Maui.Services;

namespace SecureConnection.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<APIService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}