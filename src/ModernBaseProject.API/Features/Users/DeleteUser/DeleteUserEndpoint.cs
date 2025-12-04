using MediatR;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.DeleteUser;

public static class DeleteUserEndpoint
{
    public static void MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/{id}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteUserCommand(id));
            return Results.NoContent();
        })
        .RequireAuthorization(Permissions.UserDelete)
        .WithName("DeleteUser")
        .WithTags("Users")
        .Produces(204);
    }
}
