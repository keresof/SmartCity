using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Common.Interfaces;

public interface IModuleRegistration
{
    void RegisterModule(IServiceCollection services, IConfiguration configuration);
}
