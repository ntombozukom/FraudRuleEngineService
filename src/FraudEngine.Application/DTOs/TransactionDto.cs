using FraudEngine.Domain.Enums;

namespace FraudEngine.Application.DTOs;

public class TransactionDto
{
    public Guid TransactionReference { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
}
