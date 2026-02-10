using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FraudEngine.Application.DTOs;

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
        var request = new UpdateFraudRuleRequest { IsEnabled = false };
        var response = await _client.PutAsJsonAsync("/api/fraud-rules/NonExistentRule", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
