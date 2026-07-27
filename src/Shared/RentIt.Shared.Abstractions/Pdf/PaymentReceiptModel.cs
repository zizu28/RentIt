namespace RentIt.Shared.Abstractions.Pdf;

public class PaymentReceiptModel
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    
    // Customer Details
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    
    // Payment Details
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    
    // Items
    public List<PaymentItemModel> Items { get; set; } = new();
    
    public decimal TotalAmount => Items.Sum(x => x.Total);
}

public class PaymentItemModel
{
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total => UnitPrice * Quantity;
}
