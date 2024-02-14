using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account;

public class ConfirmPhoneViewModel
{
    [Required]
    public string PhoneNumber { get; set; } = null!;
    
    [Required]
    public string Code { get; set; } = null!;
}