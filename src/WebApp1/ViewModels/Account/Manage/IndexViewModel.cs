using WebApp1.Enums;

namespace WebApp1.ViewModels.Account.Manage;

public class IndexViewModel
{
    public string Username { get; set; } = null!;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string? City { get; set; }

    public ActivityTypes? ActivityTypes { get; set; }
}