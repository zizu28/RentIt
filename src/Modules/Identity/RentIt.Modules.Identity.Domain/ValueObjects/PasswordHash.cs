namespace RentIt.Modules.Identity.Domain.ValueObjects;

public sealed record PasswordHash
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password hash cannot be empty.", nameof(value));
        }

        return new PasswordHash(value);
    }
}
