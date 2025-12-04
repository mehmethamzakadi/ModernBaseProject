using ModernBaseProject.Core.Domain.Common;

namespace ModernBaseProject.Core.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
