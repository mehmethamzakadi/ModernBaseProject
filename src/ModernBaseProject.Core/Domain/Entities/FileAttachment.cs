using ModernBaseProject.Core.Domain.Common;

namespace ModernBaseProject.Core.Domain.Entities;

public class FileAttachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Path { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
}
