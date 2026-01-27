using System.Text.RegularExpressions;

namespace RentIt.Modules.Identity.Domain.ValueObjects;

/// <summary>
/// Phone number value object for Ghana phone numbers
/// </summary>
public sealed record PhoneNumber
{
    private static readonly Regex GhanaPhoneRegex = new(
        @"^\+233[0-9]{9}$",
        RegexOptions.Compiled);

    public string Value { get; init; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        var normalized = NormalizePhoneNumber(phoneNumber);

        if (!GhanaPhoneRegex.IsMatch(normalized))
            throw new ArgumentException("Phone number must be in Ghana format (+233XXXXXXXXX)", nameof(phoneNumber));

        return new PhoneNumber(normalized);
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        // Remove spaces, dashes, and parentheses
        var cleaned = Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");

        // Convert 0XXXXXXXXX to +233XXXXXXXXX
        if (cleaned.StartsWith("0") && cleaned.Length == 10)
        {
            cleaned = "+233" + cleaned.Substring(1);
        }

        // Add +233 if missing
        if (!cleaned.StartsWith("+"))
        {
            if (cleaned.StartsWith("233"))
                cleaned = "+" + cleaned;
        }

        return cleaned;
    }

    public override string ToString() => Value;
}
