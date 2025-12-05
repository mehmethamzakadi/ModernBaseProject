namespace ModernBaseProject.Core.Constants;

/// <summary>
/// Configuration section keys
/// </summary>
public static class ConfigurationKeys
{
    public const string DefaultConnection = "DefaultConnection";

    public static class Jwt
    {
        public const string Key = "Jwt:Key";
        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";
    }

    public static class Seq
    {
        public const string Url = "Seq:Url";
    }
}

