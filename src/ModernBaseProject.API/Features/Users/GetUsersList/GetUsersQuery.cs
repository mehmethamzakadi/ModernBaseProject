using MediatR;

namespace ModernBaseProject.API.Features.Users.GetUsersList;

public record GetUsersQuery : IRequest<List<UserDto>>;

public record UserDto(Guid Id, string Username, string Email, bool IsActive);
