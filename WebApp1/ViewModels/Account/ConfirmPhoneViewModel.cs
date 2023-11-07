using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account;

public class ConfirmPhoneViewModel
{
    [Required]
    public int Code { get; set; } = 0;
}