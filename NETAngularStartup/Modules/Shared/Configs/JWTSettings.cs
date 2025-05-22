namespace DevCodeArchitect.Utilities;

public class JWTSettings
{
    public static string SecretKey { get; set; } = string.Empty;

    public static string Issuer { get; set;} = string.Empty;

    public static string Audience { get; set;} = string.Empty;

    public static JWTExpirationSettings Expiration { get; set; } = new JWTExpirationSettings();

}

public class JWTExpirationSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
