﻿namespace MvcBoilerplate
{
    using System;
    using Framework;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MvcBoilerplate.Commands;
    using MvcBoilerplate.Repositories;
    using MvcBoilerplate.Settings;
    using MvcBoilerplate.Translators;
    using MvcBoilerplate.ViewModels;

    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures caching for the application. Registers the <see cref="IDistrbutedCache"/> and
        /// <see cref="IMemoryCache"/> types with the services collection or IoC container. The
        /// <see cref="IDistrbutedCache"/> is intended to be used in cloud hosted scenarios where there is a shared
        /// cache, which is shared between multiple instances of the application. Use the <see cref="IMemoryCache"/>
        /// otherwise.
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            return services
                // Adds IMemoryCache which is a simple in-memory cache.
                .AddMemoryCache()
                // Adds IDistributedCache which is a distributed cache shared between multiple servers. This adds a
                // default implementation of IDistributedCache which is not distributed. See below:
                .AddDistributedMemoryCache();
            // Uncomment the following line to use the Redis implementation of IDistributedCache. This will
            // override any previously registered IDistributedCache service.
            // Redis is a very fast cache provider and the recommended distributed cache provider.
            // .AddDistributedRedisCache(
            //     options =>
            //     {
            //     });
            // Uncomment the following line to use the Microsoft SQL Server implementation of IDistributedCache.
            // Note that this would require setting up the session state database.
            // Redis is the preferred cache implementation but you can use SQL Server if you don't have an alternative.
            // .AddSqlServerCache(
            //     x =>
            //     {
            //         x.ConnectionString = "Server=.;Database=ASPNET5SessionState;Trusted_Connection=True;";
            //         x.SchemaName = "dbo";
            //         x.TableName = "Sessions";
            //     });
        }

        /// <summary>
        /// Configures the settings by binding the contents of the config.json file to the specified Plain Old CLR
        /// Objects (POCO) and adding <see cref="IOptions{}"/> objects to the services collection.
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        /// <param name="configuration">Gets or sets the application configuration, where key value pair settings are
        /// stored.</param>
        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                // Adds IOptions<CacheProfileSettings> to the services container.
                .Configure<CacheProfileSettings>(configuration.GetSection(nameof(CacheProfileSettings)));
        }

#if (CORS)
        /// <summary>
        /// Add cross-origin resource sharing (CORS) services and configures named CORS policies. See
        /// https://docs.asp.net/en/latest/security/cors.html
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        public static IServiceCollection AddCorsPolicies(IServiceCollection services)
        {
            return services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        options.DefaultPolicyName,
                        x =>
                        {
                            x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials();
                        });
                    options.AddPolicy(
                        "MyCustomPolicy",
                        x =>
                        {
                        });
                });
        }

#endif
        /// <summary>
        /// Adds project commands.
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            return services
                .AddScoped<IDeleteCarCommand, DeleteCarCommand>()
                .AddScoped(x => new Lazy<IDeleteCarCommand>(() => x.GetRequiredService<IDeleteCarCommand>()))
                .AddScoped<IGetCarCommand, GetCarCommand>()
                .AddScoped(x => new Lazy<IGetCarCommand>(() => x.GetRequiredService<IGetCarCommand>()))
                .AddScoped<IPatchCarCommand, PatchCarCommand>()
                .AddScoped(x => new Lazy<IPatchCarCommand>(() => x.GetRequiredService<IPatchCarCommand>()))
                .AddScoped<IPostCarCommand, PostCarCommand>()
                .AddScoped(x => new Lazy<IPostCarCommand>(() => x.GetRequiredService<IPostCarCommand>()))
                .AddScoped<IPutCarCommand, PutCarCommand>()
                .AddScoped(x => new Lazy<IPutCarCommand>(() => x.GetRequiredService<IPutCarCommand>()));

            // Singleton - Only one instance is ever created and returned.
            // services.AddSingleton<IExampleService, ExampleService>();

            // Scoped - A new instance is created and returned for each request/response cycle.
            // services.AddScoped<IExampleService, ExampleService>();

            // Transient - A new instance is created and returned each time.
            // services.AddTransient<IExampleService, ExampleService>();
        }

        /// <summary>
        /// Adds project repositories.
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<ICarRepository, CarRepository>();
        }

        /// <summary>
        /// Adds project translators.
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        public static IServiceCollection AddTranslators(this IServiceCollection services)
        {
            return services
                .AddSingleton<ITranslator<Models.Car, Car>, CarToCarTranslator>()
                .AddSingleton<ITranslator<Car, Models.Car>, CarToCarTranslator>()
                .AddSingleton<ITranslator<Models.Car, SaveCar>, CarToSaveCarTranslator>()
                .AddSingleton<ITranslator<SaveCar, Models.Car>, CarToSaveCarTranslator>();
        }
    }
}
