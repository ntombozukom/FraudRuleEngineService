using FluentAssertions;
using Moq;
using FraudEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Application.Tests;

public class GetAuditLogsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock;
    private readonly GetAuditLogsQueryHandler _handler;

    public GetAuditLogsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        _unitOfWorkMock.Setup(u => u.AuditLogs).Returns(_auditLogRepositoryMock.Object);
        _handler = new GetAuditLogsQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedAuditLogs()
    {
        var auditLogs = new List<AuditLog>
        {
            new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = "FraudRuleConfiguration",
                EntityId = Guid.NewGuid().ToString(),
                EntityName = "HighValueTransaction",
                Action = AuditAction.Updated,
                PropertyName = "Parameters",
                OldValue = "{\"ThresholdAmount\": 50000}",
                NewValue = "{\"ThresholdAmount\": 100000}",
                ModifiedBy = "admin@capitec.co.za",
                ModifiedAt = DateTime.UtcNow
            }
        };

        _auditLogRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<AuditAction?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<AuditLog>
            {
                Items = auditLogs,
                Page = 1,
                PageSize = 20,
                TotalCount = 1
            });

        var query = new GetAuditLogsQuery { Page = 1, PageSize = 20 };
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.Items.First().EntityName.Should().Be("HighValueTransaction");
        result.Items.First().Action.Should().Be("Updated");
    }

    [Fact]
    public async Task Handle_ShouldFilterByEntityType()
    {
        _auditLogRepositoryMock.Setup(r => r.GetAllAsync(
            "FraudRuleConfiguration",
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<AuditAction?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<AuditLog>
            {
                Items = new List<AuditLog>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            });

        var query = new GetAuditLogsQuery
        {
            EntityType = "FraudRuleConfiguration",
            Page = 1,
            PageSize = 20
        };

        await _handler.Handle(query, CancellationToken.None);

        _auditLogRepositoryMock.Verify(r => r.GetAllAsync(
            "FraudRuleConfiguration",
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<AuditAction?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            1,
            20,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterByModifiedBy()
    {
        _auditLogRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            "admin@capitec.co.za",
            It.IsAny<AuditAction?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<AuditLog>
            {
                Items = new List<AuditLog>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            });

        var query = new GetAuditLogsQuery
        {
            ModifiedBy = "admin@capitec.co.za",
            Page = 1,
            PageSize = 20
        };

        await _handler.Handle(query, CancellationToken.None);

        _auditLogRepositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            "admin@capitec.co.za",
            It.IsAny<AuditAction?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            1,
            20,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterByAction()
    {
        _auditLogRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            AuditAction.Disabled,
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<AuditLog>
            {
                Items = new List<AuditLog>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            });

        var query = new GetAuditLogsQuery
        {
            Action = AuditAction.Disabled,
            Page = 1,
            PageSize = 20
        };

        await _handler.Handle(query, CancellationToken.None);

        _auditLogRepositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            AuditAction.Disabled,
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            1,
            20,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterByDateRange()
    {
        var from = new DateTime(2025, 1, 1);
        var to = new DateTime(2025, 12, 31);

        _auditLogRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<AuditAction?>(),
            from,
            to,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<AuditLog>
            {
                Items = new List<AuditLog>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            });

        var query = new GetAuditLogsQuery
        {
            From = from,
            To = to,
            Page = 1,
            PageSize = 20
        };

        await _handler.Handle(query, CancellationToken.None);

        _auditLogRepositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<AuditAction?>(),
            from,
            to,
            1,
            20,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyWhenNoLogs()
    {
        _auditLogRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<AuditAction?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<AuditLog>
            {
                Items = new List<AuditLog>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            });

        var query = new GetAuditLogsQuery { Page = 1, PageSize = 20 };
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
