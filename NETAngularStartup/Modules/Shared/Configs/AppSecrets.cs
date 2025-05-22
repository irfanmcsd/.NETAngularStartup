namespace DevCodeArchitect.Utilities;

public class AppSecrets
{
    public static AppDBSecrets Database { get; set; } = new AppDBSecrets();
    public static AppAWSSecrets Aws { get; set; } = new AppAWSSecrets();
    public static OAuthSecrets OAuth { get; set; } = new OAuthSecrets();
    public static AppPaymentSecrets Payment { get; set;} = new AppPaymentSecrets();
    public static MapSecrets Map { get; set; } = new MapSecrets();

    /// <summary>
    /// Passing these keys to app - to perform actions on UI level henever needed. e.g render google map required api key
    /// </summary>
    /// <returns>
    /// Object containing selected credential settings (currently empty)
    /// </returns>
    public static object GetSettings()
    {
        return new
        {
            GoogleMapKey = Map.Google.API_Key,
            GoogleClientID = OAuth.Google.ClientId,
            Stripe_SiteKey = Payment.Stripe.SITE_KEY
        };
    }
}

public class AppDBSecrets
{
    public string Host { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set;} = string.Empty;
}

public class AppPaymentSecrets
{
    public AppPaymentStripeSecrts Stripe { get; set; } = new AppPaymentStripeSecrts();
}

public class AppPaymentStripeSecrts
{
    public string SITE_KEY { get; set; } = string.Empty;
    public string SECRETE_KEY { get; set;} = string.Empty;
}

public class AppAWSSecrets
{
    public string AccessKeyId {  get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;


}

public class OAuthSecrets
{
    public OAuthGoogleSecrets Google { get; set; } = new OAuthGoogleSecrets();

    public OAuthTwitterSecrets Twitter { get; set; } = new OAuthTwitterSecrets();

    public OAuthFacebookSecrets Facebook { get; set;} = new OAuthFacebookSecrets();
}

public class OAuthGoogleSecrets
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set;} = string.Empty;
}

public class OAuthTwitterSecrets
{
     public string ConsumerKey { get; set;} = string.Empty;
     public string ConsumerSecret { get; set; } = string.Empty;
}

public class OAuthFacebookSecrets
{
    public string FB_ID { get; set; } = string.Empty;
    public string FB_SECRET { get; set; } = string.Empty;
}

public class MapSecrets
{
    public GoogleMapSecrets Google { get; set;} = new GoogleMapSecrets();
    public MapBoxSecrets MapBox { get; set; } = new MapBoxSecrets();
}

public class GoogleMapSecrets
{
    public string API_Key { get; set;} = string.Empty;
}

public class MapBoxSecrets
{
    public string MAPBOXACCESS_TOKEN { get; set; } = string.Empty;
}