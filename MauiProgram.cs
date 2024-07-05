using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

#if ANDROID
using UsbApp.Droid;
#elif WINDOWS
using UsbApp.WinUI;
#elif IOS
using UsbApp.iOS;
#elif MACCATALYST
using UsbApp.MacCatalyst;
#endif


namespace UsbApp
{
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
                })
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
        {
#if ANDROID
            mauiAppBuilder.Services.AddSingleton<IUsbSerialService, UsbSerialService_Android>();
#elif WINDOWS
            mauiAppBuilder.Services.AddSingleton<IUsbSerialService, UsbSerialService_Windows>();
#elif IOS
            mauiAppBuilder.Services.AddSingleton<IUsbSerialService, UsbSerialService_iOS>();
#elif MACCATALYST
            mauiAppBuilder.Services.AddSingleton<IUsbSerialService, UsbSerialService_MacCatalyst>();
#endif
            // More services registered here.

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            //mauiAppBuilder.Services.AddSingleton<MainPageViewModel>();
            // More view-models registered here.

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainPage>();
            // More views registered here.

            return mauiAppBuilder;
        }

    }
}
