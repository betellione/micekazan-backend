using WebApp1.Models;
using WebApp1.ViewModels;
using WebApp1.ViewModels.Event;

namespace WebApp1.Mapping;

public static class UserMap
{
    public static UserViewModel UserMapping(this User user)
    {
        return new UserViewModel { Id = user.Id, Email = user.Email!, ExpiresAt = user.ExpiresAt, };
    }

    public static Scanner MapToScanner(this User user)
    {
        return new Scanner
        {
            Id = user.Id,
            Username = user.UserName!,
        };
    }
}