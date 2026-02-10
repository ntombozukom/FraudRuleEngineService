namespace FraudEngine.Application.DTOs;

public class UpdateFraudRuleRequest
{
    public bool? IsEnabled { get; set; }
    public string? Parameters { get; set; }
}
