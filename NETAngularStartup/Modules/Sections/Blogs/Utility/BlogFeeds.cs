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
                          <loc>{{feed.Url}}</loc>
                          <lastmod>{{feed.UpdatedAt}}</lastmod>
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
                          <link href="{{feed.Url}}">{{feed.Url}}</link>
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