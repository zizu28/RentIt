namespace RentIt.Client.Web.Services;

public class InitializePaymentRequestDto
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}

public class InitializePaymentResponseDto
{
    public string AuthorizationUrl { get; set; } = string.Empty;
}

public interface IPaymentService
{
    Task<InitializePaymentResponseDto> InitializePaymentAsync(InitializePaymentRequestDto request);
}
