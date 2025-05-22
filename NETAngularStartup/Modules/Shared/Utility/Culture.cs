namespace DevCodeArchitect.Utilities;

public class CultureUtil
{
    public static string[] SupportedCultures()
    {
        return new[] { "en", "fr", "de", "it", "pt", "ko", "ja", "ar", "ru", "zh", "es" };
    }

    public static List<KeyValuePair<string, string>> SupportedCultureList()
    {
        return new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("en", "English"),
            new KeyValuePair<string, string>("fr", "French"),
            new KeyValuePair<string, string>("de", "German"),
            new KeyValuePair<string, string>("it", "Italian"),
            new KeyValuePair<string, string>("pt", "Portuguese"),
            new KeyValuePair<string, string>("ko", "Korean"),
            new KeyValuePair<string, string>("ja", "Japanese"),
            new KeyValuePair<string, string>("ar", "Arabic"),
            new KeyValuePair<string, string>("ru", "Russian"),
            new KeyValuePair<string, string>("zh", "Chinese"),
            new KeyValuePair<string, string>("es", "Spanish")
        };
    }

}
