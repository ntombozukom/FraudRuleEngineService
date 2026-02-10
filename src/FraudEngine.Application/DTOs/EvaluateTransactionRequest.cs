using FraudEngine.Domain.Enums;

namespace FraudEngine.Application.DTOs;

public class EvaluateTransactionRequest
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string MerchantName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Country { get; set; } = "ZA";
    public TransactionChannel Channel { get; set; }
}
