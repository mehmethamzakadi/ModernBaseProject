using ModernBaseProject.Core.Domain.Common;

namespace ModernBaseProject.Core.Domain.Entities;

public class Permission : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
