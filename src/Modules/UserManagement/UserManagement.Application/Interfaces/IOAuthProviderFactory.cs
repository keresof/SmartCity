namespace UserManagement.Application.Interfaces;

public interface IOAuthProviderFactory
{
    IOAuthProvider GetProvider(string providerName);
}