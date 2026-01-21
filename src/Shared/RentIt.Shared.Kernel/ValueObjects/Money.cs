using RentIt.Shared.Kernel.Enums;

namespace RentIt.Shared.Kernel.ValueObjects;

/// <summary>
/// Money value object representing an amount and currency
/// </summary>
public sealed record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        return new Money(amount, currency);
    }

    public static Money Zero(Currency currency) => new(0, currency);

    public static Money Cedis(decimal amount) => Create(amount, Currency.GHS);
    public static Money Dollars(decimal amount) => Create(amount, Currency.USD);
    public static Money Euros(decimal amount) => Create(amount, Currency.EUR);
    public static Money Pounds(decimal amount) => Create(amount, Currency.GBP);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add money with different currencies. Expected {Currency}, got {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract money with different currencies. Expected {Currency}, got {other.Currency}");

        if (Amount < other.Amount)
            throw new InvalidOperationException("Subtraction would result in negative amount");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));

        return new Money(Amount * multiplier, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new ArgumentException("Divisor must be greater than zero", nameof(divisor));

        return new Money(Amount / divisor, Currency);
    }

    public bool IsZero() => Amount == 0;

    public bool IsPositive() => Amount > 0;

    public override string ToString() => $"{Amount:N2} {Currency}";

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);
    public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);
}
