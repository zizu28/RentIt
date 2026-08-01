namespace RentIt.Modules.Payments.Application.Services;

public class InitializeTransactionRequest
{
    public string Email { get; set; } = string.Empty;
    public decimal Amount { get; set; } // Paystack requires amount in kobo/cents. I'll pass decimal and the service will multiply by 100.
    public string Reference { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}

public class InitializeTransactionResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public InitializeTransactionData Data { get; set; } = new();
}

public class InitializeTransactionData
{
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string AccessCode { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}

public interface IPaystackService
{
    Task<InitializeTransactionResponse> InitializeTransactionAsync(InitializeTransactionRequest request, CancellationToken cancellationToken = default);
}
