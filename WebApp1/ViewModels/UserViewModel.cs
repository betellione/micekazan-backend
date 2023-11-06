using System.ComponentModel.DataAnnotations;

namespace WebApp1.ViewModels;

public class UserViewModel
{
    public Guid Id { get; set; }
    [Display(Name = "Email")]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }
    public string? EventName { get; set; }
    public long? EventId { get; set; }
}