using RentIt.Modules.Payments.Domain.Enums;

namespace RentIt.Modules.Payments.Domain.ValueObjects;

public record PaymentMethod(
    string Provider,
    PaymentMethodType MethodType,
    string Last4,
    int? ExpiryMonth,
    int? ExpiryYear,
    string EncryptedProviderToken
);
