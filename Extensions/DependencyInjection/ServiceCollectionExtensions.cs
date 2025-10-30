using Microsoft.Extensions.DependencyInjection;
using Saphira.Discord.EventSubscriber;
using System.Reflection;

namespace Saphira.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
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

        foreach (var type in subscriberTypes)
        {
            var subscriber = (IDiscordSocketClientEventSubscriber)serviceProvider.GetRequiredService(type);
            subscriber.Register();
        }
    }
}
