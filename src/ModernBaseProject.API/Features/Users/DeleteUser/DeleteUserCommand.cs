using MediatR;

namespace ModernBaseProject.API.Features.Users.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
