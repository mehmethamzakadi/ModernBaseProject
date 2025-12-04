using Microsoft.AspNetCore.Authorization;

namespace ModernBaseProject.Infrastructure.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}
