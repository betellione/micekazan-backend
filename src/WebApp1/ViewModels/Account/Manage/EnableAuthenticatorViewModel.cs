using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account.Manage;

public class EnableAuthenticatorViewModel
{
    [Required]
    [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Verification Code")]
    public string Code { get; set; } = null!;
}