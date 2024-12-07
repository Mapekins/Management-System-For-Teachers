using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClassLibrary;
using static Management_System_For_Teachers.MauiProgram;

namespace Management_System_For_Teachers
{
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
            // Pievieno ceļu uz appsetings.json
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile(AppContext.BaseDirectory + @"..\..\..\..\..\..\" + "appsettings.json")
            .AddEnvironmentVariables()
            .Build();
            //Serializē appsetings.json klasē “ConnectionStrings”
            ConnectionStrings? dbPath = config.GetRequiredSection("ConnectionStrings").Get<ConnectionStrings>();
            //Datu bāzes inicializēšana
            DatabaseHandler.Initialize(dbPath.SQLiteDBPath);
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        //klase ConnectionStrings, kurā ir ceļš uz datubāzi
        internal class ConnectionStrings
        {
            public string SQLiteDBPath { get; set; }
        }
    }
}
