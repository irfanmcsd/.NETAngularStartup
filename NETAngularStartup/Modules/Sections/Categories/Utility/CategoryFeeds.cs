using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

using System.Text;

namespace DevCodeArchitect.SDK;
public class CategoryFeeds
{
    public static async Task<string> generateGoogleSitemap(ApplicationDBContext context, CategoryQueryEntity entity, string directory = "doc")
    {
        var str = new StringBuilder();

        str.Append($$"""
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
            """);

        if (entity != null)
        {
            var feeds = await CategoriesBLL.LoadItems(context, entity);
            if (feeds != null)
            {
                // 
                foreach (var feed in feeds)
                {
                    var term = feed.Term ?? string.Empty;
                    if (feed.UpdatedAt == null)
                        feed.UpdatedAt = DateTime.Now;

                    str.Append($$"""
                        <url>
                           <loc>{{CategoryUrls.GetPostUrl("en", directory, term)}}</loc>
                           
                           <lastmod>{{feed.UpdatedAt}}</lastmod>

                           <xhtml:link rel="alternate" hreflang="en" href="{{CategoryUrls.GetPostUrl("en", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="de" href="{{CategoryUrls.GetPostUrl("de", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="fr" href="{{CategoryUrls.GetPostUrl("fr", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="it" href="{{CategoryUrls.GetPostUrl("it", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="es" href="{{CategoryUrls.GetPostUrl("es", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="ru" href="{{CategoryUrls.GetPostUrl("ru", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="zh" href="{{CategoryUrls.GetPostUrl("zh", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="ja" href="{{CategoryUrls.GetPostUrl("ja", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="ko" href="{{CategoryUrls.GetPostUrl("ko", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="pt" href="{{CategoryUrls.GetPostUrl("pt", directory, term)}}" />
                           <xhtml:link rel="alternate" hreflang="ar" href="{{CategoryUrls.GetPostUrl("ar", directory, term)}}" />

                           <!-- Optional: x-default for fallback -->
                           <xhtml:link rel="alternate" hreflang="x-default" href="{{CategoryUrls.GetPostUrl("en", directory, term)}}" />
                        </url>
                        """);
                }
            }
        }

        str.Append("</urlset>");

        return str.ToString();
    }

}