using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModernBaseProject.API.Features.Users.Login;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.RefreshToken;

public static class RefreshTokenEndpoint
{
    public static void MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.RefreshToken, async ([FromBody] RefreshTokenCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .WithName("RefreshToken")
        .WithTags("Auth")
        .Produces<LoginResponse>(200);
    }
}
