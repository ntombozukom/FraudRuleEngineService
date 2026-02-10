using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Application.Mappings;

public static class TransactionMappingExtensions
{
    public static TransactionDto ToDto(this Transaction transaction) => new()
    {
        TransactionReference = transaction.TransactionReference,
        AccountNumber = transaction.AccountNumber,
        Amount = transaction.Amount,
        Currency = transaction.Currency,
        MerchantName = transaction.MerchantName,
        Category = transaction.Category,
        TransactionDate = transaction.TransactionDate,
        Location = transaction.Location,
        Country = transaction.Country,
        Channel = transaction.Channel.ToString()
    };

    public static IEnumerable<TransactionDto> ToDto(this IEnumerable<Transaction> transactions) =>
        transactions.Select(t => t.ToDto());
}
