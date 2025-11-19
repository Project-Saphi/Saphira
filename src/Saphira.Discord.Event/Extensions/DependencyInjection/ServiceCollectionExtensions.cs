using Discord;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Logging;
using System.Reflection;

namespace Saphira.Discord.Event.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSubscribers(this IServiceCollection services, bool requireAttribute = true)
    {
        var finder = new TypeFinder(Assembly.GetExecutingAssembly())
            .ByInterface(typeof(IDiscordSocketClientEventSubscriber))
            .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

        foreach (var type in finder.Find())
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterEventSubscribers(this IServiceProvider serviceProvider, bool requireAttribute = true)
    {
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        var finder = new TypeFinder(Assembly.GetExecutingAssembly())
            .ByInterface(typeof(IDiscordSocketClientEventSubscriber))
            .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

        foreach (var type in finder.Find())
        {
            var subscriber = (IDiscordSocketClientEventSubscriber)serviceProvider.GetRequiredService(type);
            subscriber.Register();

            logger.Log(LogSeverity.Verbose, "Saphira", $"Registered event subscriber {subscriber.ToString()}");
        }
    }
}
