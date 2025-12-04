using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Domain.Entities;

namespace ModernBaseProject.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasMany(e => e.Roles).WithMany(e => e.Users)
                .UsingEntity<Dictionary<string, object>>("UserRole",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"));
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });
            entity.HasOne(e => e.Role).WithMany(r => r.RolePermissions).HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.Permission).WithMany(p => p.RolePermissions).HasForeignKey(e => e.PermissionId);
        });

        modelBuilder.Entity<FileAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
