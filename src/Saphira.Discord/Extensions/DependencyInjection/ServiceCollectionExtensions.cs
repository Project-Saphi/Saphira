using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Interaction.Autocompletion.ValueProvider;
using System.Reflection;

namespace Saphira.Discord.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValueProviders(this IServiceCollection services, bool requireAttribute = true)
    {
        var finder = new TypeFinder(Assembly.GetExecutingAssembly())
            .ByInterface(typeof(IValueProvider))
            .ByAttribute(typeof(AutoRegisterAttribute), requireAttribute);

        foreach (var type in finder.Find())
        {
            services.AddTransient(type);
        }

        return services;
    }
}
