using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Common;

namespace FraudEngine.API.Tests;

public class FraudAlertsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FraudAlertsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn_Ok()
    {
        var response = await _client.GetAsync("/api/fraud-alerts");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_ShouldReturn_NotFound_WhenAlertDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/fraud-alerts/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStatistics_ShouldReturn_Ok()
    {
        var response = await _client.GetAsync("/api/fraud-alerts/statistics");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<FraudAlertStatisticsDto>();
        result.Should().NotBeNull();
    }
}
