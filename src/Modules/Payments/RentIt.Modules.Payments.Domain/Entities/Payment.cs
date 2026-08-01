using System.Text.Json.Serialization;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Exceptions;

namespace RentIt.Modules.Payments.Domain.Entities;

public class Payment
{
#pragma warning disable
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public string? AuthorizationUrl { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
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

    public void MarkAsSuccessful()
    {
        if (Status == PaymentStatus.Successful)
            return;

        Status = PaymentStatus.Successful;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status == PaymentStatus.Successful)
            throw new PaymentDomainException("Cannot fail an already successful payment.");

        Status = PaymentStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    private static string GenerateReference()
    {
        return $"REF-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";
    }
}
