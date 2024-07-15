namespace Shared.Common;

public interface ICanMigrate
{
    void ApplyMigrations(IServiceProvider? services);
}
