using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

using System.Text;
using System.Xml;

namespace DevCodeArchitect.SDK;
public class CategoryFeeds
{
    public static async Task<string> generateGoogleSitemap(ApplicationDBContext context, CategoryQueryEntity entity, string directory = "doc")
    {
        using var stringWriter = new StringWriter();
        var settings = new XmlWriterSettings
        {
            Indent = true,
            Async = true,
            Encoding = Encoding.UTF8
        };

        using (var writer = XmlWriter.Create(stringWriter, settings))
        {
            await writer.WriteStartDocumentAsync();

            // Root element with required namespaces
            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            await writer.WriteAttributeStringAsync("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");

            if (entity != null)
            {
                var feeds = await CategoriesBLL.LoadItems(context, entity);
                if (feeds != null)
                {
                    var languages = new[] { "en", "de", "fr", "it", "es", "ru", "zh", "ja", "ko", "pt", "ar" };

                    foreach (var feed in feeds)
                    {
                        var term = feed.Term ?? string.Empty;
                        var lastMod = feed.UpdatedAt ?? DateTime.UtcNow;

                        await writer.WriteStartElementAsync(null, "url", null);

                        // URL location
                        await writer.WriteElementStringAsync(null, "loc", null,
                            CategoryUrls.GetPostUrl("en", directory, term));

                        // Last modified (UTC format)
                        await writer.WriteElementStringAsync(null, "lastmod", null,
                            lastMod.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));

                        // Alternate language links
                        foreach (var lang in languages)
                        {
                            await writer.WriteStartElementAsync("xhtml", "link", "http://www.w3.org/1999/xhtml");
                            await writer.WriteAttributeStringAsync(null, "rel", null, "alternate");
                            await writer.WriteAttributeStringAsync(null, "hreflang", null, lang);
                            await writer.WriteAttributeStringAsync(null, "href", null,
                                CategoryUrls.GetPostUrl(lang, directory, term));
                            await writer.WriteEndElementAsync();
                        }

                        // x-default language
                        await writer.WriteStartElementAsync("xhtml", "link", "http://www.w3.org/1999/xhtml");
                        await writer.WriteAttributeStringAsync(null, "rel", null, "alternate");
                        await writer.WriteAttributeStringAsync(null, "hreflang", null, "x-default");
                        await writer.WriteAttributeStringAsync(null, "href", null,
                            CategoryUrls.GetPostUrl("en", directory, term));
                        await writer.WriteEndElementAsync();

                        await writer.WriteEndElementAsync(); // </url>
                    }
                }
            }

            await writer.WriteEndElementAsync(); // </urlset>
            await writer.WriteEndDocumentAsync();
        }

        return stringWriter.ToString();
    }

}