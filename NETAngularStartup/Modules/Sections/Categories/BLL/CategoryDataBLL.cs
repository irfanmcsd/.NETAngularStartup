using DevCodeArchitect.DBContext;
using Microsoft.EntityFrameworkCore;
namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for managing many-to-many relationships between categories and contents.
/// Handles association and dissociation of content with one or more categories.
/// </summary>
public static class CategoryDataBLL
{

    public static async Task ProcessCategoryData(ApplicationDBContext context, List<CategoryData>? CultureCategories, int CategoryId, bool IsUpdate)
    {
        if (!IsUpdate)
        {
            // Add Operation
            if (CultureCategories != null && CultureCategories.Count > 0)
            {
                foreach (var cultureItem in CultureCategories)
                {
                    if (!await CheckCultureExist(context, CategoryId, cultureItem.Culture))
                    {
                        await AddContent(context, CategoryId, cultureItem);
                    }
                }
            }
        }
        else
        {
            // Update Operation
            if (CultureCategories == null || CultureCategories.Count == 0)
            {
                // No input records
                // Delete all records if exist
                await DeleteByCategoryId(context, CategoryId);
                return;
            }

            // Load existing data for current category id
            var DbCultures = await FetchCultures(context, CategoryId);

            if (DbCultures == null || DbCultures.Count == 0)
            {
                // No record exist in db
                // add directly
                foreach (var cultureItem in CultureCategories)
                {
                    if (!await CheckCultureExist(context, CategoryId, cultureItem.Culture))
                    {
                        await AddContent(context, CategoryId, cultureItem);
                    }
                }
                return;
            }

            // Remove associations not in the new list
            foreach (var existingAssociation in DbCultures)
            {
                if (!Exists(context, CultureCategories, existingAssociation.Culture))
                {
                    await DeleteById(context, existingAssociation.Id);
                }
            }

            // Add new associations
            foreach (var NewCulture in CultureCategories)
            {
                if (!Exists(context, DbCultures, NewCulture.Culture))
                {
                    // Add newly added culture
                    await AddContent(context, CategoryId, NewCulture);
                }
                else
                {
                    // Update existing culture info
                    await UpdateCategoryData(context, NewCulture);
                }
            }
        }
    }



    public static bool Exists(ApplicationDBContext context, List<CategoryData>? List, string Culture)
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

        var itemsToDelete = context.CategoryData
            .Where(x => x.Id == Id);

        context.CategoryData.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    public static async Task DeleteByCategoryId(ApplicationDBContext context, int categoryId)
    {
        if (categoryId <= 0) return;

        var itemsToDelete = context.CategoryData
            .Where(x => x.CategoryId == categoryId);

        context.CategoryData.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    public static async Task<List<CategoryData>?> FetchCultures(
       ApplicationDBContext context,
       int CategoryId)
    {
        if (CategoryId <= 0) return null;

        return await context.CategoryData
            .Where(p => p.CategoryId == CategoryId)
            .ToListAsync();
    }

    /// <summary>
    /// Adds localized content for a category
    /// </summary>
    private static async Task<CategoryData?> AddContent(
        ApplicationDBContext context,
        int categoryId,
        CategoryData? entity)
    {
        if (entity == null) return null;

        try
        {
            var newContent = new CategoryData
            {
                CategoryId = categoryId,
                Title = entity.Title,
                Culture = entity.Culture,
                Description = entity.Description,
                MetaDescription = entity.MetaDescription,
                SubTitle = entity.SubTitle
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
    private static async Task UpdateCategoryData(
        ApplicationDBContext context,
        CategoryData entity)
    {
        if (entity.Id <= 0) return;

        try
        {
            var existingContent = await context.CategoryData
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (existingContent != null)
            {
                existingContent.Title = entity.Title;
                existingContent.SubTitle = entity.SubTitle;
                existingContent.Description = entity.Description;
                existingContent.MetaDescription = entity.MetaDescription;

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
        int CategoryId,
        string Culture)
    {
        var Records = await context.CategoryData
                .Where(p => p.CategoryId == CategoryId && p.Culture == Culture).CountAsync();
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