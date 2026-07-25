namespace RentIt.Modules.Identity.Application.Abstractions;

public interface ISocialAuthServiceFactory
{
    ISocialAuthService Create(string provider);
}
