using System;
using System.Security.Authentication.ExtendedProtection;
using Application;
using Application.Coins;
using Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            var application = serviceProvider.GetRequiredService<VendingApplication>();

            application.Run();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<VendingApplication, VendingApplication>()
                .AddScoped<IVendingMachine, VendingMachine>()
                .AddScoped<IProductDispenser, ProductDispenser>()
                .AddScoped<ICoinDetector, UsaCoinDetector>()
                ;

            return services.BuildServiceProvider();
        }
    }
}
