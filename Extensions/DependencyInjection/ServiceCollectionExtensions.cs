using Discord;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Commands.Autocompletion.ValueProvider;
using Saphira.Cronjobs;
using Saphira.Discord.EventSubscriber;
using Saphira.Util.Logging;
using System.Reflection;

namespace Saphira.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronjobs(this IServiceCollection services, bool requireAttribute = true)
    {
        foreach (var type in GetTypesImplementing(typeof(ICronjob), requireAttribute))
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterCronjobs(this IServiceProvider serviceProvider, bool requireAttribute = true)
    {
        var cronjobScheduler = serviceProvider.GetRequiredService<CronjobScheduler>();
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var type in GetTypesImplementing(typeof(ICronjob), requireAttribute))
        {
            var cronjob = (ICronjob)serviceProvider.GetRequiredService(type);
            cronjobScheduler.RegisterCronjob(cronjob);

            logger.Log(LogSeverity.Verbose, "Saphira", $"Registered cronjob {cronjob.ToString()}");
        }
    }

    public static IServiceCollection AddEventSubscribers(this IServiceCollection services, bool requireAttribute = true)
    {
        foreach (var type in GetTypesImplementing(typeof(IDiscordSocketClientEventSubscriber), requireAttribute))
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterEventSubscribers(this IServiceProvider serviceProvider, bool requireAttribute = true)
    {
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        foreach (var type in GetTypesImplementing(typeof(IDiscordSocketClientEventSubscriber), requireAttribute))
        {
            var subscriber = (IDiscordSocketClientEventSubscriber)serviceProvider.GetRequiredService(type);
            subscriber.Register();

            logger.Log(LogSeverity.Verbose, "Saphira", $"Registered event subscriber {subscriber.ToString()}");
        }
    }

    public static IServiceCollection AddValueProviders(this IServiceCollection services, bool requireAttribute = true)
    {
        foreach (var type in GetTypesImplementing(typeof(IValueProvider), requireAttribute))
        {
            services.AddTransient(type);
        }

        return services;
    }

    private static IEnumerable<Type> GetTypesImplementing(Type interfaceType, bool requireAttribute = true)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var attributeType = typeof(AutoRegisterAttribute);

        return assembly.GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t)
                     && t is { IsInterface: false, IsAbstract: false }
                     && (!requireAttribute || t.GetCustomAttribute(attributeType) != null));
    }
}
