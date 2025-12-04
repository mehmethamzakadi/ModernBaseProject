using MediatR;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.GetUserById;

public static class GetUserByIdEndpoint
{
    public static void MapGetUserByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id}", async (Guid id, IMediator mediator) =>
        {
            var response = await mediator.Send(new GetUserByIdQuery(id));
            return Results.Ok(response);
        })
        .RequireAuthorization(Permissions.UserRead)
        .WithName("GetUserById")
        .WithTags("Users")
        .Produces<UserDetailDto>(200);
    }
}
