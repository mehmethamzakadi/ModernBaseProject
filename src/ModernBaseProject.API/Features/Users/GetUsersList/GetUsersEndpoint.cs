using MediatR;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.GetUsersList;

public static class GetUsersEndpoint
{
    public static void MapGetUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetUsersQuery());
            return Results.Ok(response);
        })
        .WithName("GetUsers")
        .WithTags("Users")
        .RequireAuthorization(Permissions.UserRead)
        .Produces<List<UserDto>>(200);
    }
}
