namespace WebApp1.Models;

public class UserConfirmationPhoneCall
{
    public Guid UserId { get; set; }
    public string UserPhoneNumber { get; set; } = null!;
    public string ConfirmationToken { get; set; } = null!;
    public string ConfirmationPhoneCode { get; set; } = null!;
    public DateTime Timestamp { get; set; }

    public User User { get; set; } = null!;
}