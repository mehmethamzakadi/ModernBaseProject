using MediatR;

namespace ModernBaseProject.API.Features.Files.UploadFile;

public record UploadFileCommand(IFormFile File) : IRequest<UploadFileResponse>;
