namespace RentIt.Shared.Abstractions.Results;

/// <summary>
/// Represents an error with a code and message
/// </summary>
public sealed record Error
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error Failure(string code, string message) => new(code, message);

    public static Error Unauthorized(string code, string message) => new(code, message);
    
    public static Error NotFound(string code, string message) => new(code, message);
    
    public static Error Validation(string code, string message) => new(code, message);
    
    public static Error Conflict(string code, string message) => new(code, message);

    public static implicit operator string(Error error) => error.Code;
}
