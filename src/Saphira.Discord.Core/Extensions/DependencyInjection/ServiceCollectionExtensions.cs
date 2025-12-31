using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Core.Interaction.Autocompletion.ValueProvider;

namespace Saphira.Discord.Core.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValueProviders(this IServiceCollection services, bool requireAttribute = true)
    {
        using var provider = services.BuildServiceProvider();
        var application = provider.GetRequiredService<Application>();

        foreach (var assembly in application.LoadAssemblies())
        {
            var finder = new TypeFinder(assembly)
                        .ByInterface(typeof(IValueProvider))
                        .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

            foreach (var type in finder.Find())
            {
                services.AddTransient(type);
            }
        }

        return services;
    }
}
