using Microsoft.AspNetCore.Authorization;

namespace ModernBaseProject.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissionsClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == "permissions")?.Value;

        if (!string.IsNullOrEmpty(permissionsClaim))
        {
            var permissions = permissionsClaim.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
