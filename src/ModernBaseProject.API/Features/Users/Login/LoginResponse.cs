namespace ModernBaseProject.API.Features.Users.Login;

public record LoginResponse(string AccessToken, string RefreshToken, string Email, string Username);
