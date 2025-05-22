using DevCodeArchitect.DBContext;
using Microsoft.EntityFrameworkCore;
namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for managing many-to-many relationships between categories and contents.
/// Handles association and dissociation of content with one or more categories.
/// </summary>
public static class BlogDataBLL
{

    public static async Task ProcessBlogData(ApplicationDBContext context, List<BlogData>? CultureBlogs, int BlogId, bool IsUpdate)
    {
        if (!IsUpdate)
        {
            // Add Operation
            if (CultureBlogs != null && CultureBlogs.Count > 0)
            {
                foreach (var cultureItem in CultureBlogs)
                {
                    if (!await CheckCultureExist(context, BlogId, cultureItem.Culture))
                    {
                        await AddContent(context, BlogId, cultureItem);
                    }
                }
            }
        }
        else
        {
            // Update Operation
            if (CultureBlogs == null || CultureBlogs.Count == 0)
            {
                // No input records
                // Delete all records if exist
                await DeleteByBlogId(context, BlogId);
                return;
            }

            // Load existing data for current blog id
            var DbCultures = await FetchCultures(context, BlogId);

            if (DbCultures == null || DbCultures.Count == 0)
            {
                // No record exist in db
                // add directly
                foreach (var cultureItem in CultureBlogs)
                {
                    if (!await CheckCultureExist(context, BlogId, cultureItem.Culture))
                    {
                        await AddContent(context, BlogId, cultureItem);
                    }
                }
                return;
            }

            // Remove associations not in the new list
            foreach (var existingAssociation in DbCultures)
            {
                if (!Exists(context, CultureBlogs, existingAssociation.Culture))
                {
                    await DeleteById(context, existingAssociation.Id);
                }
            }

            // Add new associations
            foreach (var NewCulture in CultureBlogs)
            {
                if (!Exists(context, DbCultures, NewCulture.Culture))
                {
                    // Add newly added culture
                    await AddContent(context, BlogId, NewCulture);
                }
                else
                {
                    // Update existing culture info
                    await UpdateBlogData(context, NewCulture);
                }
            }
        }
    }



    public static bool Exists(ApplicationDBContext context, List<BlogData>? List, string Culture)
    {
        if (List == null || List.Count == 0) { return false; }
        foreach (var CultureItem in List)
        {
            if (CultureItem.Culture == Culture)
            {
                return true;
            }
        }
        return false;

    }

    public static async Task DeleteById(ApplicationDBContext context, int Id)
    {
        if (Id <= 0) return;

        var itemsToDelete = context.BlogData
            .Where(x => x.Id == Id);

        context.BlogData.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    public static async Task DeleteByBlogId(ApplicationDBContext context, int BlogId)
    {
        if (BlogId <= 0) return;

        var itemsToDelete = context.BlogData
            .Where(x => x.BlogId == BlogId);

        context.BlogData.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    public static async Task<List<BlogData>?> FetchCultures(
       ApplicationDBContext context,
       int BlogId)
    {
        if (BlogId <= 0) return null;

        return await context.BlogData
            .Where(p => p.BlogId == BlogId)
            .ToListAsync();
    }

    /// <summary>
    /// Adds localized content for a category
    /// </summary>
    private static async Task<BlogData?> AddContent(
        ApplicationDBContext context,
        int BlogId,
        BlogData? entity)
    {
        if (entity == null) return null;

        try
        {
            var newContent = new BlogData
            {
                BlogId = BlogId,
                Title = entity.Title,
                Culture = entity.Culture,
                Description = entity.Description,
                ShortDescription = entity.ShortDescription
            };

            context.Entry(newContent).State = EntityState.Added;
            await context.SaveChangesAsync();
            return newContent;
        }
        catch (Exception ex)
        {
            // Log error here
            return null;
        }
    }

    /// <summary>
    /// Updates localized content for a category
    /// </summary>
    private static async Task UpdateBlogData(
        ApplicationDBContext context,
        BlogData entity)
    {
        if (entity.Id <= 0) return;

        try
        {
            var existingContent = await context.BlogData
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (existingContent != null)
            {
                existingContent.Title = entity.Title;
                existingContent.ShortDescription = entity.ShortDescription;
                existingContent.Description = entity.Description;

                context.Entry(existingContent).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error here
        }
    }

    private static async Task<bool> CheckCultureExist(
        ApplicationDBContext context,
        int BlogId,
        string Culture)
    {
        var Records = await context.BlogData
                .Where(p => p.BlogId == BlogId && p.Culture == Culture).CountAsync();
        if (Records > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}