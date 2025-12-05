using MediatR;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.GetUsersList;

public static class GetUsersEndpoint
{
    public static void MapGetUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Users, async (IMediator mediator, HttpContext context) =>
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            Console.WriteLine($"Auth Header: {authHeader}");
            Console.WriteLine($"User Identity: {context.User.Identity?.Name}");
            Console.WriteLine($"User Claims: {string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            
            var response = await mediator.Send(new GetUsersQuery());
            return Results.Ok(response);
        })
        .WithName("GetUsers")
        .WithTags("Users")
        .RequireAuthorization(Permissions.UserRead)
        .Produces<List<UserDto>>(200);
    }
}
