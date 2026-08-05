namespace RentIt.Client.Web.Models;

public class PropertyFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public int Type { get; set; } = 1;
    public int Status { get; set; }
    public int RentalPeriod { get; set; } = 2;
    public int Bedrooms { get; set; } = 1;
    public int Bathrooms { get; set; } = 1;
    public decimal PricePerPeriod { get; set; } = 50.00m;
    public decimal SecurityDeposit { get; set; } = 0.00m;
}
