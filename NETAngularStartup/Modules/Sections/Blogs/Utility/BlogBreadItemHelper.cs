using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;
public class BlogBreadItemHelper
{
    public static void Generate(BlogListViewModel listentity, BlogListingQueryModel entity)
    {
        var breadItems = new List<BreadItem>();

        // add home link [Home]
        breadItems.Add(new BreadItem() { Url = "/", Title = "Home" });

        // add Blogs link [Home | Blogs]
        breadItems.Add(new BreadItem() { Url = BlogUrls.GetUrl(entity.Culture ?? "en"), Title = "Blogs" });

        // category bread items
        processCategoryBreadItems(breadItems, entity);

        // search
        /*if (!string.IsNullOrEmpty(entity.term))
            breadItems.Add(new BreadItem() { url = PropertyUrls.getSearchUrl(entity.term), title = UtilityHelper.setTitle(entity.term) });

       
        // author posts
        if (!string.IsNullOrEmpty(entity.user_slug))
        {
            // [Home | Blogs | Author]
            breadItems.Add(new BreadItem() { url = UserUrls.getUrl(), title = "Author" });

            // [Home | Blogs | Author | Agent
            breadItems.Add(new BreadItem() { url = PropertyUrls.getAgentUrl(entity.user_slug), title = UtilityHelper.setTitle(entity.user_slug) });
        }*/

        // author posts
        if (!string.IsNullOrEmpty(entity.Label))
        {
            // [Home | Blogs | Tags]
            breadItems.Add(new BreadItem() { Url = BlogUrls.GetLabelUrl(entity.Culture ?? "en"), Title = "Tags" });

            // [Home | Blogs | Tags | Tag
            breadItems.Add(new BreadItem() { Url = BlogUrls.GetLabelUrl(entity.Culture ?? "en", entity.Label), Title = UtilityHelper.SetTitle(entity.Label) });
        }

        // Featured 
        if (entity.Featured != null)
        {
            // [Home | Blogs | Yearly, Monthly etc
            breadItems.Add(new BreadItem() { Url = BlogUrls.GetFeaturedUrl(entity.Culture ?? "en", entity.Featured), Title = UtilityHelper.SetTitle(Types.ParseFeaturedTypes(entity.Featured)) });
        }

        // page number bread items
        if (entity.PageNumber != null && entity.PageNumber > 1)
            breadItems.Add(new BreadItem() { Url = "#", Title = "Page - " + entity.PageNumber });

        listentity.BreadItems = breadItems;
    }

    private static void processCategoryBreadItems(List<BreadItem> breadItems, BlogListingQueryModel entity)
    {
        // categories
        if (!string.IsNullOrEmpty(entity.CategoryChild4))
        {
            // possible route => /category_child4/category_child3/category_child2/category_child1/categoryname
            // physcially child4 is actualy first child e.g Home (Child4) | Houses (Child3) | Apartments (Child2)
            // [Home | Blogs | For Sale | Child 4 => Home]

            // [Home (child4)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl(entity.Culture ?? "en", entity.CategoryChild4),
                Title = UtilityHelper.SetTitle(entity.CategoryChild4)
            });

            // [Home (child4) | House (child3)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild4, entity.CategoryChild3),
                Title = UtilityHelper.SetTitle(entity.CategoryChild3)
            });

            // [Home (child4) | House (child3) | Apartment (child2)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild4, entity.CategoryChild3, entity.CategoryChild2),
                Title = UtilityHelper.SetTitle(entity.CategoryChild2)
            });

            // [Home (child4) | House (child3) | Apartment (child2) | Sub Apartment (child1)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild4, entity.CategoryChild3, entity.CategoryChild2, entity.CategoryChild1),
                Title = UtilityHelper.SetTitle(entity.CategoryChild1)
            });

            // [Home (child4) | House (child3) | Apartment (child2) | Sub Apartment (child1) | Sub Sub Apartment (categoryname) ]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild4, entity.CategoryChild3, entity.CategoryChild2, entity.CategoryChild1, entity.CategoryName),
                Title = UtilityHelper.SetTitle(entity.CategoryName)
            });

        }
        else if (!string.IsNullOrEmpty(entity.CategoryChild3))
        {
            // [Home (child3)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl(entity.Culture ?? "en", entity.CategoryChild3),
                Title = UtilityHelper.SetTitle(entity.CategoryChild3)
            });

            // [Home (child3) | House (child2)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild3, entity.CategoryChild2),
                Title = UtilityHelper.SetTitle(entity.CategoryChild2)
            });

            // [Home (child3) | House (child2) | Apartment (child1)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild3, entity.CategoryChild2, entity.CategoryChild1),
                Title = UtilityHelper.SetTitle(entity.CategoryChild1)
            });

            // [Home (child3) | House (child2) | Apartment (child1) | Sub Apartment (categoryname)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild3, entity.CategoryChild2, entity.CategoryChild1, entity.CategoryName),
                Title = UtilityHelper.SetTitle(entity.CategoryName)
            });
        }
        else if (!string.IsNullOrEmpty(entity.CategoryChild2))
        {
            // [Home (child2)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl(entity.Culture ?? "en", entity.CategoryChild2),
                Title = UtilityHelper.SetTitle(entity.CategoryChild2)
            });

            // [Home (child2) | House (child1)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild2, entity.CategoryChild1),
                Title = UtilityHelper.SetTitle(entity.CategoryChild1)
            });

            // [Home (child2) | House (child1) | Apartment (categoryname)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild2, entity.CategoryChild1, entity.CategoryName),
                Title = UtilityHelper.SetTitle(entity.CategoryName)
            });
        }
        else if (!string.IsNullOrEmpty(entity.CategoryChild1))
        {
            // [Home (child1)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl(entity.Culture ?? "en", entity.CategoryChild1),
                Title = UtilityHelper.SetTitle(entity.CategoryChild1)
            });

            // [Home (child1) | House (categoryname)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl( entity.CategoryChild1, entity.CategoryName),
                Title = UtilityHelper.SetTitle(entity.CategoryName)
            });

        }
        else if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            // [Home (categoryname)]
            breadItems.Add(new BreadItem()
            {
                Url = BlogUrls.GetCategoryUrl(entity.Culture ?? "en", entity.CategoryName),
                Title = UtilityHelper.SetTitle(entity.CategoryName)
            });
        }
    }


}

/* Copyright © 2025, Mediasoftpro All rights reserved.
 * For inquiries and more information, please contact us at:
 * Email: clouddevarchitect@outlook.com
 * Website: www.devcodearchitect.com
 */