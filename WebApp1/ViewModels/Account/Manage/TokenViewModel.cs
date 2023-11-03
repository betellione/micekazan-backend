using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account.Manage;

public class TokenViewModel
{
    [Required]
    [Display(Name = "New token")]
    public string Token { get; set; } = null!;
}