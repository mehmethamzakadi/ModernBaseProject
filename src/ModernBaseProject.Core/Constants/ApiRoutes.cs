namespace ModernBaseProject.Core.Constants;

/// <summary>
/// API endpoint route paths
/// </summary>
public static class ApiRoutes
{
    public const string Base = "/api";

    // Auth Routes
    public const string Auth = Base + "/auth";
    public const string Login = Auth + "/login";
    public const string RefreshToken = Auth + "/refresh";

    // User Routes
    public const string Users = Base + "/users";
    public const string UsersById = Users + "/{id}";

    // File Routes
    public const string Files = Base + "/files";
    public const string FilesUpload = Files + "/upload";

    // Role Routes
    public const string Roles = Base + "/roles";

    // SignalR Hubs
    public const string SignalR = "/hubs";
    public const string NotificationsHub = SignalR + "/notifications";
}

