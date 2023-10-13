using Micekazan.Api.Types;

namespace Micekazan.Api.Models;

public class Role
{
    public RoleType Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}

