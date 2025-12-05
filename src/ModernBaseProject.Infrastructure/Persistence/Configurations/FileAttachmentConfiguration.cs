using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModernBaseProject.Core.Domain.Entities;

namespace ModernBaseProject.Infrastructure.Persistence.Configurations;

public class FileAttachmentConfiguration : BaseEntityConfiguration<FileAttachment>
{
    public override void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        base.Configure(builder);

        builder.HasKey(e => e.Id);
    }
}
