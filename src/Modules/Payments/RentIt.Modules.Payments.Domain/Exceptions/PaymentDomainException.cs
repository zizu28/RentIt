using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Payments.Domain.Exceptions;

public class PaymentDomainException(string message) : Exception(message)
{
}
