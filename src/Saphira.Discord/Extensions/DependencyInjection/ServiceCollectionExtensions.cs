using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Interaction.Foundation.Autocompletion.ValueProvider;

namespace Saphira.Discord.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValueProviders(this IServiceCollection services, bool requireAttribute = true)
    {
        foreach (var assembly in Application.LoadAssemblies())
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
