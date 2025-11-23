using Discord;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core.Cronjob;
using Saphira.Core.Event;
using Saphira.Core.Logging;

namespace Saphira.Core.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronjobs(this IServiceCollection services, bool requireAttribute = true)
    {
        using var provider = services.BuildServiceProvider();
        var application = provider.GetRequiredService<Application>();

        foreach (var assembly in application.LoadAssemblies())
        {
            var finder = new TypeFinder(assembly)
                .ByInterface(typeof(ICronjob))
                .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

            foreach (var type in finder.Find())
            {
                services.AddSingleton(type);
            }
        }

        return services;
    }

    public static void RegisterCronjobs(this IServiceProvider serviceProvider, bool requireAttribute = true)
    {
        var application = serviceProvider.GetRequiredService<Application>();
        var cronjobScheduler = serviceProvider.GetRequiredService<CronjobScheduler>();
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var assembly in application.LoadAssemblies())
        {
            var finder = new TypeFinder(assembly)
                .ByInterface(typeof(ICronjob))
                .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

            foreach (var type in finder.Find())
            {
                var cronjob = (ICronjob) serviceProvider.GetRequiredService(type);
                cronjobScheduler.RegisterCronjob(cronjob);

                logger.Log(LogSeverity.Verbose, "Saphira", $"Registered cronjob {cronjob}");
            }
        }
    }

    public static IServiceCollection AddEventSubscribers(this IServiceCollection services, bool requireAttribute = true)
    {
        using var provider = services.BuildServiceProvider();
        var application = provider.GetRequiredService<Application>();

        foreach (var assembly in application.LoadAssemblies())
        {
            var finder = new TypeFinder(assembly)
                        .ByInterface(typeof(IEventSubscriber))
                        .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

            foreach (var type in finder.Find())
            {
                services.AddSingleton(type);
            }
        }

        return services;
    }

    public static void RegisterEventSubscribers(this IServiceProvider serviceProvider, bool requireAttribute = true)
    {
        var application = serviceProvider.GetRequiredService<Application>();
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var assembly in application.LoadAssemblies())
        {
            var finder = new TypeFinder(assembly)
                        .ByInterface(typeof(IEventSubscriber))
                        .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

            foreach (var type in finder.Find())
            {
                var subscriber = (IEventSubscriber) serviceProvider.GetRequiredService(type);
                subscriber.Register();

                logger.Log(LogSeverity.Verbose, "Saphira", $"Registered event subscriber {subscriber}");
            }
        }
    }
}
