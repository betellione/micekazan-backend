using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account;

public class LoginWith2FaViewModel
{
    public string ReturnUrl { get; set; } = null!;

    [Required]
    [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Authenticator code")]
    public string TwoFactorCode { get; set; } = null!;

    public bool RememberMe { get; set; }

    [Display(Name = "Remember machine")]
    public bool RememberMachine { get; set; }
}