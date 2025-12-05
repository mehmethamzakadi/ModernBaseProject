using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Constants;
using ModernBaseProject.Core.Domain.Entities;
using ModernBaseProject.Infrastructure.Authentication;

namespace ModernBaseProject.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Seed Permissions
        var permissions = new List<Permission>
        {
            new() { Id = Guid.NewGuid(), Key = Permissions.UserCreate, Description = "Create users", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = Permissions.UserRead, Description = "Read users", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = Permissions.UserUpdate, Description = "Update users", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = Permissions.UserDelete, Description = "Delete users", CreatedAt = DateTime.UtcNow }
        };

        foreach (var permission in permissions)
        {
            if (!await context.Permissions.AnyAsync(p => p.Key == permission.Key))
                await context.Permissions.AddAsync(permission);
        }
        await context.SaveChangesAsync();

        // Seed SuperAdmin Role
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.SuperAdmin);
        if (superAdminRole == null)
        {
            superAdminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = Roles.SuperAdmin,
                CreatedAt = DateTime.UtcNow
            };
            await context.Roles.AddAsync(superAdminRole);
            await context.SaveChangesAsync();
        }

        // Assign all permissions to SuperAdmin
        var allPermissions = await context.Permissions.ToListAsync();
        foreach (var permission in allPermissions)
        {
            if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRole.Id && rp.PermissionId == permission.Id))
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id
                });
            }
        }
        await context.SaveChangesAsync();

        // Seed Admin User
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == SeederConstants.AdminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = SeederConstants.AdminUsername,
                Email = SeederConstants.AdminEmail,
                PasswordHash = PasswordHasher.HashPassword(SeederConstants.AdminPassword),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Assign SuperAdmin role to admin user
            adminUser.Roles.Add(superAdminRole);
            await context.SaveChangesAsync();
        }
    }
}
