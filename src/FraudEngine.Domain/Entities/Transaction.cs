namespace FraudEngine.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public Guid TransactionReference { get; set; } = Guid.NewGuid();
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string MerchantName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Country { get; set; } = "ZA";
    public Enums.TransactionChannel Channel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FraudAlert> FraudAlerts { get; set; } = new List<FraudAlert>();
}
