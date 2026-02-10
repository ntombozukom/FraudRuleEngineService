using FluentAssertions;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;

namespace FraudEngine.Domain.Tests;

public class TransactionTests
{
    [Fact]
    public void Transaction_ShouldInitialize_WithDefaults()
    {
        var txn = new Transaction();
        txn.Id.Should().Be(0);
        txn.TransactionReference.Should().NotBeEmpty();
        txn.Currency.Should().Be("ZAR");
        txn.Country.Should().Be("ZA");
        txn.FraudAlerts.Should().BeEmpty();
    }

    [Fact]
    public void Transaction_ShouldSet_AllProperties()
    {
        var txnRef = Guid.NewGuid();
        var txn = new Transaction
        {
            Id = 1,
            TransactionReference = txnRef,
            AccountNumber = "1234567890",
            Amount = 75000m,
            MerchantName = "Luxury Store",
            Category = "Shopping",
            TransactionDate = DateTime.UtcNow,
            Location = "Cape Town",
            Country = "ZA",
            Channel = TransactionChannel.InStore
        };

        txn.Id.Should().Be(1);
        txn.TransactionReference.Should().Be(txnRef);
        txn.Amount.Should().Be(75000m);
        txn.Channel.Should().Be(TransactionChannel.InStore);
        txn.Country.Should().Be("ZA");
    }

    [Fact]
    public void Transaction_ShouldGenerate_UniqueTransactionReference()
    {
        var txn1 = new Transaction();
        var txn2 = new Transaction();

        txn1.TransactionReference.Should().NotBe(txn2.TransactionReference);
    }
}
