using FluentAssertions;
using Moq;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Infrastructure.FraudRules;

namespace FraudEngine.Application.Tests;

public class HighValueTransactionRuleTests
{
    private readonly Mock<IFraudRuleConfigurationCache> _configCacheMock;
    private readonly HighValueTransactionRule _rule;

    public HighValueTransactionRuleTests()
    {
        _configCacheMock = new Mock<IFraudRuleConfigurationCache>();
        _rule = new HighValueTransactionRule(_configCacheMock.Object);
    }

    [Fact]
    public async Task EvaluateAsync_ShouldFlag_WhenAmountExceedsThreshold()
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("HighValueTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "HighValueTransaction",
                IsEnabled = true,
                Parameters = "{\"ThresholdAmount\": 50000}"
            });

        var txn = new Transaction { Amount = 75000m, MerchantName = "Luxury Store" };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().NotBeNull();
        result!.Severity.Should().Be(AlertSeverity.High);
        result.RuleName.Should().Be("HighValueTransaction");
    }

    [Fact]
    public async Task EvaluateAsync_ShouldNotFlag_WhenAmountBelowThreshold()
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("HighValueTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "HighValueTransaction",
                IsEnabled = true,
                Parameters = "{\"ThresholdAmount\": 50000}"
            });

        var txn = new Transaction { Amount = 1000m };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().BeNull();
    }

    [Fact]
    public async Task EvaluateAsync_ShouldNotFlag_WhenRuleDisabled()
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("HighValueTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "HighValueTransaction",
                IsEnabled = false,
                Parameters = "{\"ThresholdAmount\": 50000}"
            });

        var txn = new Transaction { Amount = 75000m };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().BeNull();
    }
}
