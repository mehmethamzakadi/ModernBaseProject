using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Login, async ([FromBody] LoginCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces<LoginResponse>(200);
    }
}
