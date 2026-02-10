using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.Transactions.Commands.EvaluateTransaction;
using FraudEngine.Domain.Enums;

namespace FraudEngine.API.Tests;

public class TransactionsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TransactionsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Evaluate_ShouldReturn_Ok_WithEvaluationResult()
    {
        var command = new EvaluateTransactionCommand
        {
            AccountNumber = "1234567890",
            Amount = 75000m,
            Currency = "ZAR",
            MerchantName = "Luxury Store",
            Category = "Shopping",
            TransactionDate = DateTime.UtcNow,
            Location = "Sandton",
            Country = "ZA",
            Channel = TransactionChannel.InStore
        };

        var response = await _client.PostAsJsonAsync("/api/transactions/evaluate", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<EvaluationResultDto>();
        result.Should().NotBeNull();
        result!.TransactionReference.Should().NotBeEmpty();
        result.IsFlagged.Should().BeTrue(); // Should be flagged by HighValueTransaction rule
        result.AlertsGenerated.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Evaluate_ShouldReturn_Ok_WhenTransactionIsClean()
    {
        var command = new EvaluateTransactionCommand
        {
            AccountNumber = "9876543210",
            Amount = 100m,
            Currency = "ZAR",
            MerchantName = "Woolworths",
            Category = "Groceries",
            TransactionDate = new DateTime(2025, 1, 15, 10, 0, 0),
            Location = "Cape Town",
            Country = "ZA",
            Channel = TransactionChannel.InStore
        };

        var response = await _client.PostAsJsonAsync("/api/transactions/evaluate", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<EvaluationResultDto>();
        result.Should().NotBeNull();
        result!.IsFlagged.Should().BeFalse();
        result.AlertsGenerated.Should().Be(0);
    }
}
