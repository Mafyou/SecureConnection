using CommunityToolkit.Maui;
using EncryptDecryptLib;
using Microsoft.Extensions.Logging;
using SecureConnection.Maui.Services;

namespace SecureConnection.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddCertificateManager();
        builder.Services.AddTransient<SymmetricEncryptDecrypt>();
        builder.Services.AddTransient<AsymmetricEncryptDecrypt>();
        builder.Services.AddTransient<DigitalSignatures>();

        builder.Services.AddSingleton<APIService>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}