using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;
public class BlogTitleHelper
{
    public static void Generate(BlogListViewModel listEntity, BlogListingQueryModel entity)
    {
        string title = BuildHeadingTitle(entity);
        listEntity.HeadingTitle = title;
    }

    private static string BuildHeadingTitle(BlogListingQueryModel entity)
    {
        // Handle featured content first (highest priority)
        if (entity.Featured == Types.FeaturedTypes.Featured)
        {
            return "Featured Blog Posts";
        }

        // Handle other content types
        if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            return BuildHeadingCategoryTitle(entity.CategoryName);
        }

        if (!string.IsNullOrEmpty(entity.Term))
        {
            return BuildHeadingSearchTitle(entity.Term);
        }

        if (!string.IsNullOrEmpty(entity.UserSlug))
        {
            return BuildHeadingAuthorTitle(entity.UserSlug);
        }

        if (!string.IsNullOrEmpty(entity.Label))
        {
            return BuildHeadingLabelTitle(entity.Label);
        }

        // Default title
        return "Blog - Latest News, Tips & Guides";
    }

    private static string BuildHeadingCategoryTitle(string categoryName)
    {
        var cleanedName = CleanText(categoryName);
        return $"Explore {cleanedName} Category";
    }

    private static string BuildHeadingSearchTitle(string term)
    {
        var cleanedTerm = CleanText(term);
        return $"Search Results for '{cleanedTerm}'";
    }

    private static string BuildHeadingAuthorTitle(string userSlug)
    {
        var cleanedSlug = CleanText(userSlug);
        var formattedName = FormatTitleCase(cleanedSlug);
        return $"Articles by {formattedName}";
    }

    private static string BuildHeadingLabelTitle(string label)
    {
        var cleanedLabel = CleanText(label);
        var formattedLabel = FormatTitleCase(cleanedLabel);
        return $"Tag: {formattedLabel}";
    }
      

    private static string FormatTitleCase(string input) =>
        UtilityHelper.SetTitle(input);

    public static void GenerateMetaTitle(BlogListViewModel listEntity, BlogListingQueryModel entity)
    {
        string title = BuildDefaultTitle();

        // Handle featured content first (highest priority)
        if (entity.Featured == Types.FeaturedTypes.Featured)
        {
            title = BuildFeaturedTitle();
        }
        // Handle other content types
        else if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            title = BuildCategoryTitle(entity.CategoryName);
        }
        else if (!string.IsNullOrEmpty(entity.Term))
        {
            title = BuildSearchTitle(entity.Term);
        }
        else if (!string.IsNullOrEmpty(entity.UserSlug))
        {
            title = BuildAuthorTitle(entity.UserSlug);
        }
        else if (!string.IsNullOrEmpty(entity.Label))
        {
            title = BuildLabelTitle(entity.Label);
        }

        listEntity.MetaTitle = title;
    }

    private static string BuildDefaultTitle() =>
        $"Blog – Latest News, Tips & Guides | {ApplicationSettings.PageCaption}";

    private static string BuildFeaturedTitle() =>
        $"Featured Blog Posts - Top Insights from {ApplicationSettings.PageCaption}";

    private static string BuildCategoryTitle(string categoryName) =>
        $"Explore {FormatTitleText(categoryName)} Category – {ApplicationSettings.PageCaption} Blog";

    private static string BuildSearchTitle(string term) =>
        $"Search Results for '{FormatText(term)}' - {ApplicationSettings.PageCaption} Blog";

    private static string BuildAuthorTitle(string userSlug) =>
        $"Articles by {FormatTitleText(userSlug)} - {ApplicationSettings.PageCaption} Blog";

    private static string BuildLabelTitle(string label) =>
        $"Tag: {FormatTitleText(label)} - Insights from {ApplicationSettings.PageCaption} Blog";

    private static string FormatText(string input) =>
        UtilityHelper.ReplaceHyphensWithSpaces(input);

    private static string FormatTitleText(string input) =>
        UtilityHelper.SetTitle(FormatText(input));

    public static void GenerateMetaDescription(BlogListViewModel listentity, BlogListingQueryModel entity)
    {
        string _meta = BuildDefaultMetaDescription();

        if (entity.Featured == Types.FeaturedTypes.Featured)
        {
            _meta = BuildFeaturedMetaDescription();
        }
        else if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            _meta = BuildCategoryMetaDescription(entity.CategoryName);
        }
        else if (!string.IsNullOrEmpty(entity.Term))
        {
            _meta = BuildSearchMetaDescription(entity.Term);
        }
        else if (!string.IsNullOrEmpty(entity.UserSlug))
        {
            _meta = BuildAuthorMetaDescription(entity.UserSlug);
        }
        else if (!string.IsNullOrEmpty(entity.Label))
        {
            _meta = BuildLabelMetaDescription(entity.Label);
        }

        listentity.MetaDescription = _meta;
    }

    private static string BuildDefaultMetaDescription() =>
        $"Explore expert articles, tips, and strategies on crypto trading bots. Stay informed with the latest insights from {ApplicationSettings.PageCaption}'s categorized blog content.";

    private static string BuildFeaturedMetaDescription() =>
        $"Explore featured blog posts from {ApplicationSettings.PageCaption}. Discover top insights, expert strategies, and highlights on AI-powered crypto trading and automation.";

    private static string BuildCategoryMetaDescription(string categoryName) =>
        $"Discover in-depth blog posts on {CleanText(categoryName)} from {ApplicationSettings.PageCaption}. Learn strategies, trends, and tips to enhance your crypto trading automation.";

    private static string BuildSearchMetaDescription(string term) =>
        $"Find blog posts matching '{CleanText(term)}' on {ApplicationSettings.PageCaption}. Explore guides, insights, and tutorials on running AI crypto trading bots on your own server.";

    private static string BuildAuthorMetaDescription(string userSlug) =>
        $"Read expert articles by {CleanText(userSlug)} on {ApplicationSettings.PageCaption}. Discover insights, strategies, and tutorials on AI-driven crypto trading and automation.";

    private static string BuildLabelMetaDescription(string label) =>
        $"Discover in-depth blog posts on {CleanText(label)} from {ApplicationSettings.PageCaption}. Learn strategies, trends, and tips to enhance your crypto trading automation.";

    private static string CleanText(string input) =>
        UtilityHelper.ReplaceHyphensWithSpaces(input);


}