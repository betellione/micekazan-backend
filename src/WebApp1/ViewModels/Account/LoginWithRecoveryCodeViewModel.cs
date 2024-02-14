using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account;

public class LoginWithRecoveryCodeViewModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Recovery Code")]
    public string RecoveryCode { get; set; } = null!;
}