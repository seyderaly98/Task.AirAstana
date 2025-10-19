using Task.AirAstana.Domain.Common;

namespace Task.AirAstana.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public bool HasRole(string roleCode)
    {
        return Role?.Code.Equals(roleCode, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public bool IsModerator()
    {
        return HasRole("Moderator");
    }
}