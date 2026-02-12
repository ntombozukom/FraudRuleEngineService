using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Common;

namespace FraudEngine.API.Tests;

public class AuditLogsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuditLogsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn_Ok()
    {
        var response = await _client.GetAsync("/api/audit-logs?page=1&pageSize=20");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyInitially()
    {
        var response = await _client.GetAsync("/api/audit-logs?page=1&pageSize=20");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        result.Should().NotBeNull();
        result!.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetAll_ShouldFilterByEntityType()
    {
        var response = await _client.GetAsync("/api/audit-logs?entityType=FraudRuleConfiguration&page=1&pageSize=20");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByModifiedBy()
    {
        var response = await _client.GetAsync("/api/audit-logs?modifiedBy=admin@capitec.co.za&page=1&pageSize=20");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByAction()
    {
        var response = await _client.GetAsync("/api/audit-logs?action=4&page=1&pageSize=20");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByDateRange()
    {
        var response = await _client.GetAsync("/api/audit-logs?from=2025-01-01&to=2025-12-31&page=1&pageSize=20");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AuditLog_ShouldBeCreated_WhenRuleIsDisabled()
    {
        // Disable a rule
        var disableRequest = new UpdateFraudRuleRequest
        {
            IsEnabled = false,
            ModifiedBy = "audit-test@capitec.co.za"
        };
        var disableResponse = await _client.PutAsJsonAsync("/api/fraud-rules/CrossBorderTransaction", disableRequest);
        disableResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check audit log was created
        var auditResponse = await _client.GetAsync("/api/audit-logs?entityName=CrossBorderTransaction&page=1&pageSize=10");
        var auditResult = await auditResponse.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();

        auditResult.Should().NotBeNull();
        auditResult!.Items.Should().Contain(a =>
            a.EntityName == "CrossBorderTransaction" &&
            a.Action == "Disabled" &&
            a.ModifiedBy == "audit-test@capitec.co.za");
    }

    [Fact]
    public async Task AuditLog_ShouldBeCreated_WhenRuleIsEnabled()
    {
        // First disable, then enable
        var disableRequest = new UpdateFraudRuleRequest
        {
            IsEnabled = false,
            ModifiedBy = "enable-test@capitec.co.za"
        };
        await _client.PutAsJsonAsync("/api/fraud-rules/UnusualCategory", disableRequest);

        var enableRequest = new UpdateFraudRuleRequest
        {
            IsEnabled = true,
            ModifiedBy = "enable-test@capitec.co.za"
        };
        var enableResponse = await _client.PutAsJsonAsync("/api/fraud-rules/UnusualCategory", enableRequest);
        enableResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check audit log was created
        var auditResponse = await _client.GetAsync("/api/audit-logs?entityName=UnusualCategory&page=1&pageSize=10");
        var auditResult = await auditResponse.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();

        auditResult.Should().NotBeNull();
        auditResult!.Items.Should().Contain(a =>
            a.EntityName == "UnusualCategory" &&
            a.Action == "Enabled" &&
            a.ModifiedBy == "enable-test@capitec.co.za");
    }

    [Fact]
    public async Task AuditLog_ShouldBeCreated_WhenParametersUpdated()
    {
        var updateRequest = new UpdateFraudRuleRequest
        {
            Parameters = "{\"ThresholdAmount\": 75000}",
            ModifiedBy = "params-test@capitec.co.za"
        };
        var updateResponse = await _client.PutAsJsonAsync("/api/fraud-rules/HighValueTransaction", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check audit log was created
        var auditResponse = await _client.GetAsync("/api/audit-logs?entityName=HighValueTransaction&page=1&pageSize=10");
        var auditResult = await auditResponse.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();

        auditResult.Should().NotBeNull();
        auditResult!.Items.Should().Contain(a =>
            a.EntityName == "HighValueTransaction" &&
            a.Action == "Updated" &&
            a.PropertyName == "Parameters" &&
            a.ModifiedBy == "params-test@capitec.co.za");
    }

    [Fact]
    public async Task AuditLog_ShouldCaptureOldAndNewValues()
    {
        // Update parameters
        var updateRequest = new UpdateFraudRuleRequest
        {
            Parameters = "{\"StartHour\": 22, \"EndHour\": 6}",
            ModifiedBy = "values-test@capitec.co.za"
        };
        await _client.PutAsJsonAsync("/api/fraud-rules/AfterHoursTransaction", updateRequest);

        // Check audit log captures old/new values
        var auditResponse = await _client.GetAsync("/api/audit-logs?modifiedBy=values-test@capitec.co.za&page=1&pageSize=10");
        var auditResult = await auditResponse.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();

        auditResult.Should().NotBeNull();
        var log = auditResult!.Items.FirstOrDefault(a => a.ModifiedBy == "values-test@capitec.co.za");
        log.Should().NotBeNull();
        log!.OldValue.Should().NotBeNullOrEmpty();
        log.NewValue.Should().Contain("StartHour");
    }
}
