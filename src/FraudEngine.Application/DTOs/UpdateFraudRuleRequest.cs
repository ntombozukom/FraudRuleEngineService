using System.ComponentModel.DataAnnotations;

namespace FraudEngine.Application.DTOs;

public class UpdateFraudRuleRequest
{
    public bool? IsEnabled { get; set; }
    public string? Parameters { get; set; }

    [Required(ErrorMessage = "ModifiedBy is required for audit purposes")]
    [EmailAddress(ErrorMessage = "ModifiedBy must be a valid email address")]
    public string ModifiedBy { get; set; } = string.Empty;
}
