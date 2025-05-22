using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;
public class BlogTitleHelper
{
    public static void Generate(BlogListViewModel listentity, BlogListingQueryModel entity)
    {
        var _title = "Blog Posts";
       
        // category title -> only last child which is categoryname 
        if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            // {categoryname} Properties - For Sale => Apartment For Sale
            _title = UtilityHelper.SetTitle(entity.CategoryName) + " " + _title;
        }

        // search
        if (!string.IsNullOrEmpty(entity.Term))
        {
            // Property Search in "{term}" => Property Search in "Commercial Apartments in Ohio"
            _title = "Blog Search in " + UtilityHelper.SetTitle(entity.Term);
        }


        // author listings
        if (!string.IsNullOrEmpty(entity.UserSlug))
        {
            // {user_slug} Property Listings
            _title = UtilityHelper.SetTitle(entity.UserSlug) + " Blog Posts";
        }

        // label listings
        if (!string.IsNullOrEmpty(entity.Label))
        {
            // {user_slug} Property Listings
            _title = UtilityHelper.SetTitle(entity.Label) + " Blog Posts";
        }

        // Featured 
        if (entity.Featured != null)
        {
            switch (entity.Featured)
            {
                case Types.FeaturedTypes.Featured:
                    _title = "Featured Posts";
                    break;
            }

        }


        listentity.HeadingTitle = _title;
    }

}