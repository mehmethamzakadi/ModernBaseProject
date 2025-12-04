using MediatR;

namespace ModernBaseProject.API.Features.Roles.GetRolesList;

public static class GetRolesEndpoint
{
    public static void MapGetRolesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetRolesQuery());
            return Results.Ok(response);
        })
        .WithName("GetRoles")
        .WithTags("Roles")
        .RequireAuthorization()
        .Produces<List<RoleDto>>(200);
    }
}