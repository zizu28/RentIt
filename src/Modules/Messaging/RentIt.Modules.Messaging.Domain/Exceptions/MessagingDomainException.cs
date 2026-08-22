using RentIt.Shared.Abstractions.Domain;
using RentIt.Shared.Abstractions.Exceptions;

namespace RentIt.Modules.Messaging.Domain.Exceptions;

public class MessagingDomainException(string message) : Exception(message)
{
}
