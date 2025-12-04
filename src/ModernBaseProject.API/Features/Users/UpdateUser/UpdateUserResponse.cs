namespace ModernBaseProject.API.Features.Users.UpdateUser;

public record UpdateUserResponse(Guid Id, string Username, string Email, bool IsActive);
