using Shared.Common.Interfaces;

namespace Shared.Common.DependencyInjection;

public static class ModuleDiscovery
{
     public static IEnumerable<IModuleRegistration?> DiscoverModules()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModuleRegistration).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => Activator.CreateInstance(t) as IModuleRegistration);
    }    
}
