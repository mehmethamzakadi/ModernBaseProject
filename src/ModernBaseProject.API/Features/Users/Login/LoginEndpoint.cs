using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModernBaseProject.API.Features.Users.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async ([FromBody] LoginCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces<LoginResponse>(200);
    }
}
