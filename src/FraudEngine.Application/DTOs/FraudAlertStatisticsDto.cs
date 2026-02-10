namespace FraudEngine.Application.DTOs;

public class FraudAlertStatisticsDto
{
    public int TotalAlerts { get; set; }
    public Dictionary<string, int> BySeverity { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> ByRule { get; set; } = new();
}
