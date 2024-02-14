using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels.Account.Manage;

public class EmailViewModel
{
    public string? Email { get; set; }
    public bool? IsEmailConfirmed { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "New email")]
    public string NewEmail { get; set; } = null!;
}