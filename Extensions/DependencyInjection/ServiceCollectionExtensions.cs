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
        var assembly = Assembly.GetExecutingAssembly();
        var cronjobType = typeof(ICronjob);
        var attributeType = typeof(AutoRegisterAttribute);

        var cronjobTypes = assembly.GetTypes()
            .Where(t => cronjobType.IsAssignableFrom(t)
                     && t is { IsInterface: false, IsAbstract: false }
                     && (!requireAttribute || t.GetCustomAttribute(attributeType) != null));

        foreach (var type in cronjobTypes)
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterCronjobs(this IServiceProvider serviceProvider, bool requireAttribute = false)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var cronjobType = typeof(ICronjob);
        var attributeType = typeof(AutoRegisterAttribute);

        var cronjobTypes = assembly.GetTypes()
            .Where(t => cronjobType.IsAssignableFrom(t)
                     && t is { IsInterface: false, IsAbstract: false }
                     && (!requireAttribute || t.GetCustomAttribute(attributeType) != null));

        var cronjobScheduler = serviceProvider.GetRequiredService<CronjobScheduler>();
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var type in cronjobTypes)
        {
            var cronjob = (ICronjob)serviceProvider.GetRequiredService(type);
            cronjobScheduler.RegisterCronjob(cronjob);

            logger.Log(LogSeverity.Info, "Saphira", $"Registered cronjob {cronjob.ToString()}");
        }
    }

    public static IServiceCollection AddEventSubscribers(this IServiceCollection services, bool requireAttribute = false)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var subscriberType = typeof(IDiscordSocketClientEventSubscriber);
        var attributeType = typeof(AutoRegisterAttribute);

        var subscriberTypes = assembly.GetTypes()
            .Where(t => subscriberType.IsAssignableFrom(t)
                     && t is { IsInterface: false, IsAbstract: false }
                     && (!requireAttribute || t.GetCustomAttribute(attributeType) != null));

        foreach (var type in subscriberTypes)
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterEventSubscribers(this IServiceProvider serviceProvider, bool requireAttribute = false)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var subscriberType = typeof(IDiscordSocketClientEventSubscriber);
        var attributeType = typeof(AutoRegisterAttribute);

        var subscriberTypes = assembly.GetTypes()
            .Where(t => subscriberType.IsAssignableFrom(t)
                     && t is { IsInterface: false, IsAbstract: false }
                     && (!requireAttribute || t.GetCustomAttribute(attributeType) != null));

        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var type in subscriberTypes)
        {
            var subscriber = (IDiscordSocketClientEventSubscriber)serviceProvider.GetRequiredService(type);
            subscriber.Register();

            logger.Log(LogSeverity.Info, "Saphira", $"Registered event subscriber {subscriber.ToString()}");
        }
    }
}
