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

public class PaymentDetailsDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public interface IPaymentService
{
    Task<InitializePaymentResponseDto> InitializePaymentAsync(InitializePaymentRequestDto request);
    Task<PaymentDetailsDto?> GetPaymentByBookingIdAsync(Guid bookingId);
}
