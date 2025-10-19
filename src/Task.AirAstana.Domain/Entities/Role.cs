using Task.AirAstana.Domain.Common;

namespace Task.AirAstana.Domain.Entities;

public class Role : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}