using Discord;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Cronjobs;
using Saphira.Discord.EventSubscriber;
using Saphira.Util.Logging;
using System.Reflection;

namespace Saphira.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronjobs(this IServiceCollection services, bool requireAttribute = false)
    {
        foreach (var type in GetTypesImplementing(typeof(ICronjob), requireAttribute))
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterCronjobs(this IServiceProvider serviceProvider, bool requireAttribute = false)
    {
        var cronjobScheduler = serviceProvider.GetRequiredService<CronjobScheduler>();
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var type in GetTypesImplementing(typeof(ICronjob), requireAttribute))
        {
            var cronjob = (ICronjob)serviceProvider.GetRequiredService(type);
            cronjobScheduler.RegisterCronjob(cronjob);

            logger.Log(LogSeverity.Info, "Saphira", $"Registered cronjob {cronjob.ToString()}");
        }
    }

    public static IServiceCollection AddEventSubscribers(this IServiceCollection services, bool requireAttribute = false)
    {
        foreach (var type in GetTypesImplementing(typeof(IDiscordSocketClientEventSubscriber), requireAttribute))
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterEventSubscribers(this IServiceProvider serviceProvider, bool requireAttribute = false)
    {
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var type in GetTypesImplementing(typeof(IDiscordSocketClientEventSubscriber), requireAttribute))
        {
            var subscriber = (IDiscordSocketClientEventSubscriber)serviceProvider.GetRequiredService(type);
            subscriber.Register();

            logger.Log(LogSeverity.Info, "Saphira", $"Registered event subscriber {subscriber.ToString()}");
        }
    }

    private static IEnumerable<Type> GetTypesImplementing(Type interfaceType, bool requireAttribute = false)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var attributeType = typeof(AutoRegisterAttribute);

        return assembly.GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t)
                     && t is { IsInterface: false, IsAbstract: false }
                     && (!requireAttribute || t.GetCustomAttribute(attributeType) != null));
    }
}
