using FluentAssertions;
using Moq;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Infrastructure.FraudRules;

namespace FraudEngine.Application.Tests;

public class CrossBorderTransactionRuleTests
{
    private readonly Mock<IFraudRuleConfigurationCache> _configCacheMock;
    private readonly CrossBorderTransactionRule _rule;

    public CrossBorderTransactionRuleTests()
    {
        _configCacheMock = new Mock<IFraudRuleConfigurationCache>();
        _rule = new CrossBorderTransactionRule(_configCacheMock.Object);
    }

    [Fact]
    public async Task EvaluateAsync_ShouldFlag_WhenCountryIsNotHomeCountry()
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("CrossBorderTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "CrossBorderTransaction",
                IsEnabled = true,
                Parameters = "{\"HomeCountry\": \"ZA\"}"
            });

        var txn = new Transaction { Country = "NG", MerchantName = "Lagos Store" };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().NotBeNull();
        result!.Severity.Should().Be(AlertSeverity.Medium);
    }

    [Fact]
    public async Task EvaluateAsync_ShouldNotFlag_WhenCountryIsHomeCountry()
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("CrossBorderTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "CrossBorderTransaction",
                IsEnabled = true,
                Parameters = "{\"HomeCountry\": \"ZA\"}"
            });

        var txn = new Transaction { Country = "ZA" };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().BeNull();
    }
}
