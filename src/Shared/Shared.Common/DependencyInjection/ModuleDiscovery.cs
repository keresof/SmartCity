using Shared.Common.Interfaces;
using System.Reflection;

namespace Shared.Common.DependencyInjection;
public static class ModuleDiscovery
{
    public static IEnumerable<T> DiscoverModules<T>(string? basePath = null) where T : class
    {
        basePath ??= AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"Scanning for modules of type {typeof(T).FullName} in: {basePath}");

        var assemblies = LoadAssemblies(basePath);
        return DiscoverModulesFromAssemblies<T>(assemblies);
    }

    private static IEnumerable<Assembly> LoadAssemblies(string basePath)
    {
        var assemblies = new List<Assembly>();

        // Load assemblies from the base directory
        foreach (var file in Directory.GetFiles(basePath, "*.dll"))
        {
            TryLoadAssembly(file, assemblies);
        }

        // Scan Modules directory if it exists
        var modulesPath = Path.Combine(basePath, "Modules");
        if (Directory.Exists(modulesPath))
        {
            foreach (var moduleDir in Directory.GetDirectories(modulesPath))
            {
                foreach (var file in Directory.GetFiles(moduleDir, "*.dll"))
                {
                    TryLoadAssembly(file, assemblies);
                }
            }
        }

        Console.WriteLine($"Loaded {assemblies.Count} assemblies");
        return assemblies;
    }

    private static void TryLoadAssembly(string file, List<Assembly> assemblies)
    {
        try
        {
            var assembly = Assembly.LoadFrom(file);
            assemblies.Add(assembly);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading assembly {file}: {ex.Message}");
        }
    }

    private static IEnumerable<T> DiscoverModulesFromAssemblies<T>(IEnumerable<Assembly> assemblies) where T : class
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    Console.WriteLine($"Found module type: {type.FullName}");
                    if (Activator.CreateInstance(type) is T module)
                    {
                        yield return module;
                    }
                }
            }
        }
    }
}