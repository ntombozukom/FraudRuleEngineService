using FluentAssertions;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;

namespace FraudEngine.Domain.Tests;

public class AuditLogTests
{
    [Fact]
    public void AuditLog_ShouldInitializeWithDefaults()
    {
        var auditLog = new AuditLog();

        auditLog.Id.Should().Be(Guid.Empty);
        auditLog.EntityType.Should().BeEmpty();
        auditLog.EntityId.Should().BeEmpty();
        auditLog.EntityName.Should().BeEmpty();
        auditLog.ModifiedBy.Should().BeEmpty();
        auditLog.ModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AuditLog_ShouldSetPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var entityId = Guid.NewGuid().ToString();
        var modifiedAt = DateTime.UtcNow;

        var auditLog = new AuditLog
        {
            Id = id,
            EntityType = "FraudRuleConfiguration",
            EntityId = entityId,
            EntityName = "HighValueTransaction",
            Action = AuditAction.Updated,
            PropertyName = "Parameters",
            OldValue = "{\"ThresholdAmount\": 50000}",
            NewValue = "{\"ThresholdAmount\": 100000}",
            ModifiedBy = "admin@capitec.co.za",
            ModifiedAt = modifiedAt
        };

        auditLog.Id.Should().Be(id);
        auditLog.EntityType.Should().Be("FraudRuleConfiguration");
        auditLog.EntityId.Should().Be(entityId);
        auditLog.EntityName.Should().Be("HighValueTransaction");
        auditLog.Action.Should().Be(AuditAction.Updated);
        auditLog.PropertyName.Should().Be("Parameters");
        auditLog.OldValue.Should().Be("{\"ThresholdAmount\": 50000}");
        auditLog.NewValue.Should().Be("{\"ThresholdAmount\": 100000}");
        auditLog.ModifiedBy.Should().Be("admin@capitec.co.za");
        auditLog.ModifiedAt.Should().Be(modifiedAt);
    }

    [Fact]
    public void AuditLog_ShouldAllowNullableProperties()
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "FraudRuleConfiguration",
            EntityId = Guid.NewGuid().ToString(),
            EntityName = "TestRule",
            Action = AuditAction.Enabled,
            PropertyName = null,
            OldValue = null,
            NewValue = null,
            ModifiedBy = "admin@capitec.co.za",
            ModifiedAt = DateTime.UtcNow
        };

        auditLog.PropertyName.Should().BeNull();
        auditLog.OldValue.Should().BeNull();
        auditLog.NewValue.Should().BeNull();
    }
}

public class AuditActionTests
{
    [Theory]
    [InlineData(AuditAction.Created, 0)]
    [InlineData(AuditAction.Updated, 1)]
    [InlineData(AuditAction.Deleted, 2)]
    [InlineData(AuditAction.Enabled, 3)]
    [InlineData(AuditAction.Disabled, 4)]
    public void AuditAction_ShouldHaveCorrectValues(AuditAction action, int expectedValue)
    {
        ((int)action).Should().Be(expectedValue);
    }

    [Fact]
    public void AuditAction_ShouldHaveFiveValues()
    {
        var values = Enum.GetValues<AuditAction>();
        values.Should().HaveCount(5);
    }

    [Theory]
    [InlineData(AuditAction.Created, "Created")]
    [InlineData(AuditAction.Updated, "Updated")]
    [InlineData(AuditAction.Deleted, "Deleted")]
    [InlineData(AuditAction.Enabled, "Enabled")]
    [InlineData(AuditAction.Disabled, "Disabled")]
    public void AuditAction_ShouldConvertToStringCorrectly(AuditAction action, string expected)
    {
        action.ToString().Should().Be(expected);
    }
}
