using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}