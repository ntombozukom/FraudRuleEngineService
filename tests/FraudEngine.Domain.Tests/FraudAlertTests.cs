using FluentAssertions;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;

namespace FraudEngine.Domain.Tests;

public class FraudAlertTests
{
    [Fact]
    public void FraudAlert_ShouldInitialize_WithOpenStatus()
    {
        var alert = new FraudAlert();
        alert.Id.Should().Be(0);
        alert.AlertReference.Should().NotBeEmpty();
        alert.Status.Should().Be(AlertStatus.Open);
    }

    [Fact]
    public void FraudAlert_ShouldSet_AllProperties()
    {
        var alertRef = Guid.NewGuid();
        var alert = new FraudAlert
        {
            Id = 1,
            AlertReference = alertRef,
            TransactionId = 42,
            RuleName = "HighValueTransaction",
            Severity = AlertSeverity.High,
            Description = "Amount exceeds threshold",
            Status = AlertStatus.UnderReview,
            ReviewedBy = "analyst@bank.co.za",
            ReviewedAt = DateTime.UtcNow
        };

        alert.Id.Should().Be(1);
        alert.AlertReference.Should().Be(alertRef);
        alert.TransactionId.Should().Be(42);
        alert.RuleName.Should().Be("HighValueTransaction");
        alert.Severity.Should().Be(AlertSeverity.High);
        alert.Status.Should().Be(AlertStatus.UnderReview);
        alert.ReviewedBy.Should().Be("analyst@bank.co.za");
        alert.ReviewedAt.Should().NotBeNull();
    }

    [Fact]
    public void FraudAlert_ShouldGenerate_UniqueAlertReference()
    {
        var alert1 = new FraudAlert();
        var alert2 = new FraudAlert();

        alert1.AlertReference.Should().NotBe(alert2.AlertReference);
    }

    [Theory]
    [InlineData(AlertSeverity.Low)]
    [InlineData(AlertSeverity.Medium)]
    [InlineData(AlertSeverity.High)]
    [InlineData(AlertSeverity.Critical)]
    public void FraudAlert_ShouldAccept_AllSeverities(AlertSeverity severity)
    {
        var alert = new FraudAlert { Severity = severity };
        alert.Severity.Should().Be(severity);
    }

    [Theory]
    [InlineData(AlertStatus.Open)]
    [InlineData(AlertStatus.UnderReview)]
    [InlineData(AlertStatus.Reviewed)]
    [InlineData(AlertStatus.Dismissed)]
    public void FraudAlert_ShouldAccept_AllStatuses(AlertStatus status)
    {
        var alert = new FraudAlert { Status = status };
        alert.Status.Should().Be(status);
    }
}
