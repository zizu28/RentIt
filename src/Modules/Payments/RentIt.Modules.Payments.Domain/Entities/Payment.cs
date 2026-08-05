using System.Text.Json.Serialization;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Events;
using RentIt.Modules.Payments.Domain.Exceptions;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Payments.Domain.Entities;

public sealed class Payment : AggregateRoot<Guid>
{
#pragma warning disable
    public Guid BookingId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal AmountPaid { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public string? AuthorizationUrl { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? EncryptedProviderToken { get; private set; }

    private Payment() { } // EF Core

    public static Payment Create(Guid bookingId, decimal amount, string currency)
    {
        if (amount <= 0)
            throw new PaymentDomainException("Amount must be greater than zero.");

        return new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Amount = amount,
            AmountPaid = 0,
            Currency = currency,
            Reference = GenerateReference(),
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetAuthorizationUrl(string authorizationUrl)
    {
        AuthorizationUrl = authorizationUrl;
    }

    public void SetProviderToken(string token)
    {
        EncryptedProviderToken = token;
    }

    public void MarkAsSuccessful()
    {
        if (Status == PaymentStatus.Successful)
            return;

        Status = PaymentStatus.Successful;
        AmountPaid = Amount;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentCompletedDomainEvent(
            Id, BookingId, Amount, Currency, "Paystack"));
    }

    public void MarkAsFailed()
    {
        if (Status == PaymentStatus.Successful)
            throw new PaymentDomainException("Cannot fail an already successful payment.");

        Status = PaymentStatus.Failed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentFailedDomainEvent(
            Id, BookingId, Amount, Currency, "Paystack"));
    }

    public void MarkAsRefunded()
    {
        if (Status != PaymentStatus.Successful && Status != PaymentStatus.PartiallyPaid)
            throw new PaymentDomainException("Only successful or partially paid payments can be refunded.");

        Status = PaymentStatus.Refunded;

        AddDomainEvent(new PaymentRefundedDomainEvent(
            Id, BookingId, AmountPaid, Currency, "Paystack"));
    }

    public void MarkAsPartiallyPaid(decimal amountPaid)
    {
        if (Status == PaymentStatus.Successful)
            throw new PaymentDomainException("Cannot partially pay an already successful payment.");

        Status = PaymentStatus.PartiallyPaid;
        AmountPaid = amountPaid;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new RentIt.Modules.Payments.Domain.Events.PaymentPartiallyPaidDomainEvent(
            Id, BookingId, amountPaid, Amount, Currency, "Paystack"));
    }

    private static string GenerateReference()
    {
        return $"REF-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";
    }
}
