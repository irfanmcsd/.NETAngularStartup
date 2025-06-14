using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DevCodeArchitect.SDK;
public class BlogFeeds
{
    public static async Task<string> generateGoogleSitemap(ApplicationDBContext context, BlogQueryEntity entity)
    {
        var str = new StringBuilder();

        str.Append($$"""
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
            """);

        if (entity != null)
        {
            var feeds = await BlogsBLL.LoadItems(context, entity);
            if (feeds != null)
            {
                foreach (var feed in feeds)
                {
                    str.Append($$"""
                        <url>
                           <loc>{{BlogUrls.GetPostUrl("en", feed.Term)}}</loc>
                           <lastmod>{{feed.UpdatedAt}}</lastmod>

                           <xhtml:link rel="alternate" hreflang="en" href="{{BlogUrls.GetPostUrl("en", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="de" href="{{BlogUrls.GetPostUrl("de", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="fr" href="{{BlogUrls.GetPostUrl("fr", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="it" href="{{BlogUrls.GetPostUrl("it", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="es" href="{{BlogUrls.GetPostUrl("es", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="ru" href="{{BlogUrls.GetPostUrl("ru", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="zh" href="{{BlogUrls.GetPostUrl("zh", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="ja" href="{{BlogUrls.GetPostUrl("ja", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="ko" href="{{BlogUrls.GetPostUrl("ko", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="pt" href="{{BlogUrls.GetPostUrl("pt", feed.Term)}}" />
                           <xhtml:link rel="alternate" hreflang="ar" href="{{BlogUrls.GetPostUrl("ar", feed.Term)}}" />

                           <!-- Optional: x-default for fallback -->
                           <xhtml:link rel="alternate" hreflang="x-default" href="{{BlogUrls.GetPostUrl("en", feed.Term)}}" />
                        </url>
                        """);
                }
            }
        }

        str.Append("</urlset>");

        return str.ToString();
    }

    public static async Task<string> generateRSS(ApplicationDBContext context, BlogQueryEntity entity)
    {
        var str = new StringBuilder();

        str.Append($$"""
            <?xml version="1.0" encoding="UTF-8" ?>
            <rss version="2.0">

            <channel>
              <title>Property Listings</title>
              <link>{{ApplicationSettings.Domain.Backend}}</link>
            """);

        if (entity != null)
        {
            var feeds = await BlogsBLL.LoadItems(context, entity);
            if (feeds != null)
            {
                foreach (var feed in feeds)
                {
                    if (feed.BlogData != null)
                    {
                        str.Append($$"""
                        <item>
                           <title>{{feed.BlogData.Title}}</title>
                           <link>{{feed.Url}}</link>
                           <description>{{WebUtility.HtmlEncode(HtmlSanitizer.StripHtml(feed.BlogData.Description))}}</description>

                           <atom:link rel="alternate" hreflang="en" href="{{BlogUrls.GetPostUrl("en", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="de" href="{{BlogUrls.GetPostUrl("de", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="fr" href="{{BlogUrls.GetPostUrl("fr", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="it" href="{{BlogUrls.GetPostUrl("it", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="es" href="{{BlogUrls.GetPostUrl("es", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="ru" href="{{BlogUrls.GetPostUrl("ru", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="zh" href="{{BlogUrls.GetPostUrl("zh", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="ja" href="{{BlogUrls.GetPostUrl("ja", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="ko" href="{{BlogUrls.GetPostUrl("ko", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="pt" href="{{BlogUrls.GetPostUrl("pt", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="ar" href="{{BlogUrls.GetPostUrl("ar", feed.Term)}}" />
                           <atom:link rel="alternate" hreflang="x-default" href="{{BlogUrls.GetPostUrl("en", feed.Term)}}" />
                        </item>
                        """);
                    }
                   
                }
            }

        }

        str.Append("""
            </channel>
            </rss>
            """);

        return str.ToString();
    }

    public static string StripHTML(string? input)
    {
        if (input == null)
            return "";

        string str = Regex.Replace(input, @"<script[^>]*?>.*?</script>", "");  // Strip out javascript 
        str = Regex.Replace(str, @"<[\/\!]*?[^<>]*?>", "");  //  Strip out HTML tags 
        str = Regex.Replace(str, @"<style[^>]*?>.*?</style>", "");  // Strip style tags properly 
        str = Regex.Replace(str, @"<![\s\S]*?--[ \t\n\r]*>", "");  // Strip multi-line comments including CDATA 
        str = Regex.Replace(str, @"\[(\w)+\](.+)\[/(\w)+\]", ""); // remove bbcode e.g [abc]...[/abc]
        return str;
    }

    public static async Task<string> generateATOM(ApplicationDBContext context, BlogQueryEntity? entity)
    {
        var str = new StringBuilder();

        str.Append($$"""
            <?xml version="1.0" encoding="utf-8"?>
            <feed xmlns="http://www.w3.org/2005/Atom">

              <title>Property Listings</title>
              <link href="{{ApplicationSettings.Domain.Backend}}"/>
              <updated>{{UtilityHelper.TimeZoneOffsetDateTime()}}</updated>
              <author>
                <name>{{ApplicationSettings.Author}}</name>
              </author>
              <id>{{Guid.NewGuid()}}</id>
            """);

        if (entity != null)
        {
            var feeds = await BlogsBLL.LoadItems(context, entity);
            if (feeds != null)
            {
                foreach (var feed in feeds)
                {
                    if (feed.BlogData != null)
                    {
                        str.Append($$"""
                        <entry>
                            <title>{{feed.BlogData.Title}}</title>

                            <!-- Primary link -->
                            <link rel="alternate" type="text/html" hreflang="en" href="{{BlogUrls.GetPostUrl("en", feed.Term)}}" />

                            <!-- Hreflang alternate links -->
                            <link rel="alternate" type="text/html" hreflang="de" href="{{BlogUrls.GetPostUrl("de", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="fr" href="{{BlogUrls.GetPostUrl("fr", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="it" href="{{BlogUrls.GetPostUrl("it", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="es" href="{{BlogUrls.GetPostUrl("es", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="ru" href="{{BlogUrls.GetPostUrl("ru", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="zh" href="{{BlogUrls.GetPostUrl("zh", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="ja" href="{{BlogUrls.GetPostUrl("ja", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="ko" href="{{BlogUrls.GetPostUrl("ko", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="pt" href="{{BlogUrls.GetPostUrl("pt", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="ar" href="{{BlogUrls.GetPostUrl("ar", feed.Term)}}" />
                            <link rel="alternate" type="text/html" hreflang="x-default" href="{{BlogUrls.GetPostUrl("en", feed.Term)}}" />

                            <id>{{feed.BlogData.Title}}</id>
                            <summary>{{WebUtility.HtmlEncode(HtmlSanitizer.StripHtml(feed.BlogData.Description))}}</summary>
                        </entry>
                        """);
                    }
                   
                }
            }

        }

        str.AppendLine("</feed>");

        return str.ToString();
    }
}