using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModernBaseProject.Core.Domain.Common;

namespace ModernBaseProject.Infrastructure.Persistence.Configurations;

/// <summary>
/// Base configuration class for all entities that inherit from BaseEntity.
/// Automatically applies soft delete query filter.
/// </summary>
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Apply soft delete query filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
