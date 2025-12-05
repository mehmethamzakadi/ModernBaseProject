using MediatR;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Files.UploadFile;

public static class UploadFileEndpoint
{
    public static void MapUploadFileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.FilesUpload, async (IFormFile file, IMediator mediator) =>
        {
            var response = await mediator.Send(new UploadFileCommand(file));
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("UploadFile")
        .WithTags("Files")
        .Produces<UploadFileResponse>(200)
        .DisableAntiforgery();
    }
}
