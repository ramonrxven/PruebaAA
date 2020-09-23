using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Abstractions;
using PruebaAA.Services;
using PruebaAA.Models;
using PruebaAA.Services.Impl;
using PruebaAA.Repository;
using PruebaAA.Repository.Impl;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PruebaAA
{
    class Program
    {
        /// <summary>
        /// Esta es la clase Entry Point de la aplicación de consola.
        ///
        /// desde el metodo Main se ejecuta la aplicación de consola,
        /// en esta clase se realiza las configuraciones generales para la aplicación
        /// ya que a diferencia de otros tipo de aplicacion de .netcore esta no cuenta
        /// con un Startup donde se puedan realizar los registros de los servicios y repositorios
        /// que utlizará la app.
        /// </summary>
        private static IServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            // var serviceProvider = services.BuildServiceProvider();
            // serviceProvider.GetService<ConsoleApp>().Run();
            // RegisterServices();
            IServiceScope scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<ConsoleApp>().Run();
            // DisposeServices();
        }

        /// <summary>
        /// Carga en el entorno de la aplicación el archivo de configuración
        /// </summary>
        /// <param name="services">instancia del punto de entrada de la aplicación</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            // ILogger logger = GetLogger();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            string connectionString = configuration.GetSection("ConnectionStrings").Value;
            services
                .AddOptions()
                .Configure<Config>(configuration.GetSection("GeneralSettings"))
                .AddTransient<ICreateTable, CreateTable>()
                .AddTransient<ISqlBulkImport, SqlBulkImport>()
                .AddTransient<IImportService, ImportService>()
                .AddTransient<ILineMappingService, LineMappingService>()
                .AddTransient<IReadFileService, ReadFileService>()
                .AddTransient<ConsoleApp>()
                .AddDbContext<PruebaAAContext>(options =>
                    {
                      options.UseSqlServer(connectionString);
                    });
            _serviceProvider = services.BuildServiceProvider(true);

        }

        // private static ILogger GetLogger()
        // {
        // var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        // ILogger logger = loggerFactory.CreateLogger("BaseLOgger");
        // return logger;
        // }

        // / <summary>
        // / registrar los servicios que se van a utilizar en la aplicacion de consola
        // /// </summary>
        // private static void RegisterServices()
        // {
        //     IServiceCollection services = new ServiceCollection() as IServiceCollection;

        //     IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
        //         .AddJsonFile(path: "AppSettings.json", optional: false, reloadOnChange: true)
        //         .Build();
        //     services
        //     .Configure(configuration.GetSection("GeneralSettings"));

        //     services.AddSingleton<IReadFileService, ReadFileService>();
        //     services.AddSingleton<ConsoleApp>();
        //     _serviceProvider = services.BuildServiceProvider(true);
        // }

        /// <summary>
        /// Este metodo ejecuta la aplicacion si se evalua que esta disponible
        /// </summary>
        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static IConfigurationRoot GetConfiguration()
        {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", optional: true)
            .Build();
        }
    }
}
