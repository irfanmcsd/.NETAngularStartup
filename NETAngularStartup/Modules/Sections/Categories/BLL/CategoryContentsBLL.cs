using DevCodeArchitect.DBContext;
using DevCodeArchitect.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for managing many-to-many relationships between categories and contents.
/// Handles association and dissociation of content with one or more categories.
/// </summary>
public static class CategoryContentsBLL
{
    #region CRUD Operations

    /// <summary>
    /// Associates a category with specified content (property, company, etc.)
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Category-content relationship to add</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task Add(ApplicationDBContext context, CategoryContents entity)
    {
        if (entity.CategoryId <= 0 || entity.ContentId <= 0) return;

        if (!await Exists(context, entity))
        {
            var newAssociation = new CategoryContents()
            {
                CategoryId = entity.CategoryId,
                ContentId = entity.ContentId,
                Type = entity.Type,
            };

            context.Entry(newAssociation).State = EntityState.Added;
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Checks if a category-content association already exists
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Category-content relationship to check</param>
    /// <returns>True if association exists, false otherwise</returns>
    public static async Task<bool> Exists(ApplicationDBContext context, CategoryContents entity)
    {
        return await context.CategoryContents
            .AnyAsync(p => p.ContentId == entity.ContentId &&
                          p.CategoryId == entity.CategoryId &&
                          p.Type == entity.Type);
    }

    /// <summary>
    /// Deletes all category associations for specified content and type
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="contentId">ID of the content</param>
    /// <param name="type">Content type</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task Delete(ApplicationDBContext context, int contentId, CategoryEnum.Types type)
    {
        if (contentId <= 0) return;

        var itemsToDelete = context.CategoryContents
            .Where(x => x.ContentId == contentId && x.Type == type);

        context.CategoryContents.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes all content associations for specified category
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="categoryId">ID of the category</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task Delete(ApplicationDBContext context, int categoryId)
    {
        if (categoryId <= 0) return;

        var itemsToDelete = context.CategoryContents
            .Where(x => x.CategoryId == categoryId);

        context.CategoryContents.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    #endregion

    #region Association Management

    /// <summary>
    /// Manages category associations for content in both Add and Update scenarios
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="categories">Array of category IDs to associate</param>
    /// <param name="contentId">ID of the content to associate with</param>
    /// <param name="type">Content type</param>
    /// <param name="isUpdate">Whether this is an update operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task ProcessAssociatedContentCategories(
        ApplicationDBContext context,
        int[]? categories,
        int contentId,
        CategoryEnum.Types type,
        bool isUpdate)
    {
        if (categories == null || contentId <= 0) return;

        var currentCategories = await FetchContentCategories(context, contentId, type);

        if (!isUpdate)
        {
            // Add new associations
            foreach (var categoryId in categories)
            {
                await Add(context, new CategoryContents()
                {
                    ContentId = contentId,
                    CategoryId = categoryId,
                    Type = type
                });
                
            }
        }
        else
        {
            await HandleUpdateScenario(context, categories, contentId, type, currentCategories);
        }
    }

    private static async Task HandleUpdateScenario(
        ApplicationDBContext context,
        int[] categories,
        int contentId,
        CategoryEnum.Types type,
        List<CategoryContents>? currentCategories)
    {
        if (currentCategories == null) return;

        if (categories.Length == 0)
        {
            // Remove all associations if no categories provided
            await Delete(context, contentId, type);
        }
        else if (currentCategories.Count == 0)
        {
            // Add all new categories if no existing associations
            foreach (var categoryId in categories)
            {
                await Add(context, new CategoryContents()
                {
                    ContentId = contentId,
                    CategoryId = categoryId,
                    Type = type
                });
               
            }
        }
        else
        {
            // Sync existing associations with new list
            await SyncCategoryAssociations(context, categories, contentId, type, currentCategories);
        }
    }

    private static async Task SyncCategoryAssociations(
        ApplicationDBContext context,
        int[] categories,
        int contentId,
        CategoryEnum.Types type,
        List<CategoryContents> currentCategories)
    {
        // Remove associations not in the new list
        foreach (var existingAssociation in currentCategories)
        {
            if (!categories.Contains(existingAssociation.CategoryId))
            {
                await DeleteAssociatedCategory(context, contentId, existingAssociation.CategoryId, type);
            }
        }

        // Add new associations
        foreach (var categoryId in categories)
        {
            if (!currentCategories.Any(c => c.CategoryId == categoryId))
            {
                await Add(context, new CategoryContents()
                {
                    ContentId = contentId,
                    CategoryId = categoryId,
                    Type = type
                });
            }
        }
    }

    #endregion

    #region Query Operations

    /// <summary>
    /// Fetches all category associations for specified content
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="contentId">ID of the content</param>
    /// <param name="type">Content type</param>
    /// <returns>List of category associations or null</returns>
    public static async Task<List<CategoryContents>?> FetchContentCategories(
        ApplicationDBContext context,
        int contentId,
        CategoryEnum.Types type)
    {
        if (contentId <= 0) return null;

        return await context.CategoryContents
            .Where(p => p.ContentId == contentId && p.Type == type)
            .ToListAsync();
    }

    /// <summary>
    /// Fetches detailed category information for specified content
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="contentId">ID of the content</param>
    /// <param name="type">Content type</param>
    /// <returns>List of category items with details or null</returns>
    public static async Task<List<CategoryItem>?> FetchContentCategoryList(
        ApplicationDBContext context,
        long contentId,
        string culture,
        CategoryEnum.Types type)
    {
        if (contentId <= 0) return null;

        var queryResult = await context.CategoryContents
            .Join(context.CategoryData,
                content => content.CategoryId,
                data => data.CategoryId,
                (content, data) => new { content, data })
            .Join(context.Categories,
                joined => joined.content.CategoryId,
                category => category.Id,
                (joined, category) => new { joined, category })
            .Where(p => p.joined.content.ContentId == contentId &&
                       p.joined.content.Type == type && 
                       p.joined.data.Culture == culture)
            .Select(p => new CategoryItem
            {
                Id = p.category.Id,
                Title = p.joined.data.Title,
                Term = p.category.Term,
                SubTerm = p.category.SubTerm
            })
            .ToListAsync();

        return queryResult;
    }

    #endregion

    #region Private Helpers

    private static async Task DeleteAssociatedCategory(
        ApplicationDBContext context,
        int contentId,
        int categoryId,
        CategoryEnum.Types type)
    {
        if (contentId <= 0 || categoryId <= 0) return;

        var itemsToDelete = context.CategoryContents
            .Where(x => x.ContentId == contentId &&
                       x.CategoryId == categoryId &&
                       x.Type == type);

        context.CategoryContents.RemoveRange(itemsToDelete);
        await context.SaveChangesAsync();
    }

    #endregion
}