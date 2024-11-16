using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.ViewModels;

public class LoginViewModel
{
    [Required]
    public string EmailAddress { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}