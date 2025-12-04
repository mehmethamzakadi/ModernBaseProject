using MediatR;

namespace ModernBaseProject.API.Features.Users.CreateUser;

public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    List<Guid> RoleIds
) : IRequest<CreateUserResponse>;
