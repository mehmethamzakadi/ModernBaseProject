using MediatR;

namespace ModernBaseProject.API.Features.Files.UploadFile;

public static class UploadFileEndpoint
{
    public static void MapUploadFileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/files/upload", async (IFormFile file, IMediator mediator) =>
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
