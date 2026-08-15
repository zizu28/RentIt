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
    public bool Status { get; set; }
}

public class PaymentDetailsDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class SetupPaymentMethodRequestDto
{
    public string Currency { get; set; } = string.Empty;
}

public class SetupPaymentMethodResponseDto
{
    public string AuthorizationUrl { get; set; } = string.Empty;
}

public class VerifyPaymentResponseDto
{
    public bool Success { get; set; }
}

public class PaymentMethodDto
{
    public string Provider { get; set; } = string.Empty;
    public string MethodType { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
}

public interface IPaymentService
{
    Task<InitializePaymentResponseDto> InitializePaymentAsync(InitializePaymentRequestDto request);
    Task<PaymentDetailsDto?> GetPaymentByBookingIdAsync(Guid bookingId);
    Task<List<PaymentMethodDto>> GetPaymentMethodsAsync();
    Task<SetupPaymentMethodResponseDto> SetupPaymentMethodAsync(SetupPaymentMethodRequestDto request);
    Task<bool> VerifyPaymentAsync(string reference);
}
