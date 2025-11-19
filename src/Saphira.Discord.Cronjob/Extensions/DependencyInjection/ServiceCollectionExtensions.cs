using Discord;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Logging;
using System.Reflection;

namespace Saphira.Discord.Cronjob.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronjobs(this IServiceCollection services, bool requireAttribute = true)
    {
        var finder = new TypeFinder(Assembly.GetExecutingAssembly())
            .ByInterface(typeof(ICronjob))
            .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

        foreach (var type in finder.Find())
        {
            services.AddSingleton(type);
        }

        return services;
    }

    public static void RegisterCronjobs(this IServiceProvider serviceProvider, bool requireAttribute = true)
    {
        var cronjobScheduler = serviceProvider.GetRequiredService<CronjobScheduler>();
        var logger = serviceProvider.GetRequiredService<IMessageLogger>();

        var finder = new TypeFinder(Assembly.GetExecutingAssembly())
            .ByInterface(typeof(ICronjob))
            .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

        foreach (var type in finder.Find())
        {
            var cronjob = (ICronjob) serviceProvider.GetRequiredService(type);
            cronjobScheduler.RegisterCronjob(cronjob);

            logger.Log(LogSeverity.Verbose, "Saphira", $"Registered cronjob {cronjob.ToString()}");
        }
    }
}
