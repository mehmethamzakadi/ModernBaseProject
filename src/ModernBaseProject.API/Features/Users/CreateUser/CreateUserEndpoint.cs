using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.API.Features.Users.CreateUser;

public static class CreateUserEndpoint
{
    public static void MapCreateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", async ([FromBody] CreateUserCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return Results.Created($"/api/users/{response.Id}", response);
        })
        .RequireAuthorization(Permissions.UserCreate)
        .WithName("CreateUser")
        .WithTags("Users")
        .Produces<CreateUserResponse>(201);
    }
}
