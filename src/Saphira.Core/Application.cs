using Discord;
using Saphira.Core.Logging;
using System.Reflection;

namespace Saphira.Core;

public class Application(IMessageLogger logger)
{
    public DateTime StartTime { get; set; }

    public List<Assembly> LoadAssemblies()
    {
        var assemblies = new List<Assembly>();

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("Saphira"))
            .ToList();

        assemblies.AddRange(loadedAssemblies);

        var assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var assemblyFiles = Directory.GetFiles(assemblyDirectory, "Saphira.*.dll");

        logger.Log(LogSeverity.Debug, "Saphira", $"Found {assemblyFiles.Length} assemblies in the current app domain");

        foreach (var assemblyFile in assemblyFiles)
        {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);

            if (loadedAssemblies.Any(a => a.FullName == assemblyName.FullName))
            {
                continue;
            }

            try
            {
                var assembly = Assembly.Load(assemblyName);
                assemblies.Add(assembly);

                logger.Log(LogSeverity.Verbose, "Saphira", $"Loaded assembly {assemblyName}");
            }
            catch
            {
                logger.Log(LogSeverity.Error, "Saphira", $"Failed to load assembly {assemblyName}");
            }
        }

        return assemblies;
    }
}
