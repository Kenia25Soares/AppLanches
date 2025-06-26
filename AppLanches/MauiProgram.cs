using AppLanches.Services;
using AppLanches.Validations;
using Microsoft.Extensions.Logging;

namespace AppLanches
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
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif      
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ApiService>();
            //builder.Services.AddSingleton<FavoritesService>();  Seria Suposto adicionar o serviço de favoritos aqui, mas não foi implementado no código fornecido.
            builder.Services.AddSingleton<IValidator, Validator>();

            return builder.Build();
        }
    }
}
