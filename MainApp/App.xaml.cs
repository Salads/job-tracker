using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDatabaseService, SQLiteService>();
            services.AddSingleton<IGeocodingService, NominatimService>();
            services.AddSingleton<IDistanceCalculatorService, HaversineDistanceCalculator>();

            return services.BuildServiceProvider();
        }
    }
}
