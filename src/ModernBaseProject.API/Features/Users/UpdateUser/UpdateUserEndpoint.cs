using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.UpdateUser;

public static class UpdateUserEndpoint
{
    public static void MapUpdateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{id}", async (Guid id, [FromBody] UpdateUserRequest request, IMediator mediator) =>
        {
            var command = new UpdateUserCommand(id, request.Username, request.Email, request.IsActive, request.RoleIds);
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .RequireAuthorization(Permissions.UserUpdate)
        .WithName("UpdateUser")
        .WithTags("Users")
        .Produces<UpdateUserResponse>(200);
    }
}

public record UpdateUserRequest(string Username, string Email, bool IsActive, List<Guid> RoleIds);
