using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.ViewModels;

public class TokenRequestViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}