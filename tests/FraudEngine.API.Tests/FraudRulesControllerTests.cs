using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Common;

namespace FraudEngine.API.Tests;

public class FraudRulesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FraudRulesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn_Ok_WithSeededRules()
    {
        var response = await _client.GetAsync("/api/fraud-rules");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rules = await response.Content.ReadFromJsonAsync<List<FraudRuleDto>>();
        rules.Should().NotBeNull();
        rules!.Count.Should().Be(5);
    }

    [Fact]
    public async Task Update_ShouldReturn_NotFound_WhenRuleDoesNotExist()
    {
        var request = new UpdateFraudRuleRequest
        {
            IsEnabled = false,
            ModifiedBy = "test@capitec.co.za"
        };
        var response = await _client.PutAsJsonAsync("/api/fraud-rules/NonExistentRule", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturn_Ok_AndCreateAuditLog()
    {
        var request = new UpdateFraudRuleRequest
        {
            IsEnabled = false,
            ModifiedBy = "test@capitec.co.za"
        };

        var response = await _client.PutAsJsonAsync("/api/fraud-rules/AfterHoursTransaction", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedRule = await response.Content.ReadFromJsonAsync<FraudRuleDto>();
        updatedRule.Should().NotBeNull();
        updatedRule!.IsEnabled.Should().BeFalse();
        updatedRule.LastModifiedBy.Should().Be("test@capitec.co.za");

        var auditResponse = await _client.GetAsync("/api/audit-logs?entityName=AfterHoursTransaction&page=1&pageSize=10");
        auditResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var auditLogs = await auditResponse.Content.ReadFromJsonAsync<PagedResponse<AuditLogDto>>();
        auditLogs.Should().NotBeNull();
        auditLogs!.Items.Should().Contain(a =>
            a.EntityName == "AfterHoursTransaction" &&
            a.ModifiedBy == "test@capitec.co.za");
    }
}
