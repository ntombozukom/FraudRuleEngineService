using FluentAssertions;
using Moq;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Infrastructure.FraudRules;

namespace FraudEngine.Application.Tests;

public class AfterHoursTransactionRuleTests
{
    private readonly Mock<IFraudRuleConfigurationCache> _configCacheMock;
    private readonly AfterHoursTransactionRule _rule;

    public AfterHoursTransactionRuleTests()
    {
        _configCacheMock = new Mock<IFraudRuleConfigurationCache>();
        _rule = new AfterHoursTransactionRule(_configCacheMock.Object);
    }

    [Theory]
    [InlineData(23)]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(4)]
    public async Task EvaluateAsync_ShouldFlag_WhenTransactionIsAfterHours(int hour)
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("AfterHoursTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "AfterHoursTransaction",
                IsEnabled = true,
                Parameters = "{\"StartHour\": 23, \"EndHour\": 5}"
            });

        var txn = new Transaction { TransactionDate = new DateTime(2025, 1, 15, hour, 30, 0) };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().NotBeNull();
        result!.Severity.Should().Be(AlertSeverity.Low);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(14)]
    [InlineData(18)]
    public async Task EvaluateAsync_ShouldNotFlag_WhenTransactionIsDuringBusinessHours(int hour)
    {
        _configCacheMock.Setup(r => r.GetByRuleNameAsync("AfterHoursTransaction", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudRuleConfiguration
            {
                RuleName = "AfterHoursTransaction",
                IsEnabled = true,
                Parameters = "{\"StartHour\": 23, \"EndHour\": 5}"
            });

        var txn = new Transaction { TransactionDate = new DateTime(2025, 1, 15, hour, 0, 0) };
        var result = await _rule.EvaluateAsync(txn);

        result.Should().BeNull();
    }
}
