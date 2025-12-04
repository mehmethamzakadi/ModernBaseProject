using MediatR;
using ModernBaseProject.Core.Domain.Entities;
using ModernBaseProject.Core.Interfaces;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Files.UploadFile;

public class UploadFileHandler : IRequestHandler<UploadFileCommand, UploadFileResponse>
{
    private readonly IFileStorageService _fileStorage;
    private readonly AppDbContext _context;

    public UploadFileHandler(IFileStorageService fileStorage, AppDbContext context)
    {
        _fileStorage = fileStorage;
        _context = context;
    }

    public async Task<UploadFileResponse> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        using var stream = request.File.OpenReadStream();
        var filePath = await _fileStorage.UploadAsync(stream, request.File.FileName, request.File.ContentType);

        var fileAttachment = new FileAttachment
        {
            FileName = request.File.FileName,
            Extension = System.IO.Path.GetExtension(request.File.FileName),
            Size = request.File.Length,
            Path = filePath,
            UploadedBy = Guid.Empty
        };

        _context.FileAttachments.Add(fileAttachment);
        await _context.SaveChangesAsync(cancellationToken);

        return new UploadFileResponse(fileAttachment.Id, fileAttachment.FileName, filePath);
    }
}
