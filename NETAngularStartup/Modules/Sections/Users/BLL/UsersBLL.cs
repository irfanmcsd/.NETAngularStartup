using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Settings;
using DevCodeArchitect.Utilities;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Text;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for user management operations
/// </summary>
/// <remarks>
/// Handles user creation, updates, deletions, queries, and various utility operations.
/// Implements caching, advanced querying, and bulk operations.
/// </remarks>
public static class UsersBLL
{
    #region User Account Operations

    /// <summary>
    /// Creates a new user account with the specified role
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="userManager">User manager service</param>
    /// <param name="roleManager">Role manager service</param>
    /// <param name="user">User entity to create</param>
    /// <param name="password">User password</param>
    /// <param name="roleName">Role to assign</param>
    /// <param name="autoActivate">Whether to automatically activate the account</param>
    /// <returns>The created user or null if failed</returns>
    public static async Task<ApplicationUser?> CreateAccount(
        ApplicationDBContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationUser user,
        string password,
        string roleName,
        bool autoActivate)
    {
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return null;

        // Ensure role exists
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new ApplicationRole
            {
                Name = roleName,
                Description = roleName,
                CreatedDate = UtilityHelper.TimeZoneOffsetDateTime(),
                IPAddress = string.Empty
            });
        }

        // Assign role
        await userManager.AddToRoleAsync(user, roleName);

        // Auto-activate if requested
        if (autoActivate)
        {
            await UpdateFieldAsync(context, user.Id, true, nameof(ApplicationUser.EmailConfirmed));
        }

        return user;
    }

    /// <summary>
    /// Updates user profile information
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User entity with updated values</param>
    public static async Task UpdateProfileAsync(ApplicationDBContext context, ApplicationUser entity)
    {
        var user = await context.AspNetUsers
            .FirstOrDefaultAsync(p => p.Id == entity.Id);

        if (user == null) return;

        // Update avatar if provided
        if (!string.IsNullOrEmpty(entity.Avatar))
        {
            user.Avatar = await UploadMedia.ProcessCover(
                context,
                entity.Avatar,
                entity.Id,
                string.Empty,
                UserSettings.AwsAvatarDirname);
        }

        // Update basic profile info
        user.FirstName = entity.FirstName;
        user.LastName = entity.LastName;
        user.PhoneNumber = entity.PhoneNumber;
        user.UserRole = entity.UserRole;

        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates user avatar information
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User entity with updated values</param>
    public static async Task UpdateAvatarAsync(ApplicationDBContext context, ApplicationUser entity)
    {
        var user = await context.AspNetUsers
            .FirstOrDefaultAsync(p => p.Id == entity.Id);

        if (user == null) return;

        // Update avatar if provided
        if (!string.IsNullOrEmpty(entity.Avatar))
        {
            user.Avatar = await UploadMedia.ProcessCover(
                context,
                entity.Avatar,
                entity.Id,
                string.Empty,
                UserSettings.AwsAvatarDirname);
        }

        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates user email information
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User entity with updated values</param>
    public static async Task UpdateEmailAsync(ApplicationDBContext context, ApplicationUser entity)
    {
        var user = await context.AspNetUsers
            .FirstOrDefaultAsync(p => p.Id == entity.Id);

        if (user == null) return;

        // Update avatar if provided
        if (!string.IsNullOrEmpty(entity.Email))
        {
            user.Email = entity.Email;

        }

        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }


    /// <summary>
    /// Activate user account
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User entity with updated values</param>
    public static async Task ActivateUserAccount(ApplicationDBContext context, ApplicationUser entity)
    {
        var user = await context.AspNetUsers
            .FirstOrDefaultAsync(p => p.Id == entity.Id);

        if (user == null) return;

        user.EmailConfirmed = true;
        user.IsEnabled = Types.ActionTypes.Enabled;

        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a user account
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User to delete</param>
    public static async Task DeleteAsync(ApplicationDBContext context, ApplicationUser entity)
    {
        if (string.IsNullOrEmpty(entity.Id)) return;

        var user = await context.AspNetUsers
            .FirstOrDefaultAsync(x => x.Id == entity.Id);

        if (user != null)
        {
            context.AspNetUsers.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Retrieves a single user record
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User identifier container</param>
    /// <returns>The user record or null if not found</returns>
    public static async Task<ApplicationUser?> GetRecordAsync(
        ApplicationDBContext context,
        ApplicationUser entity)
    {
        if (!string.IsNullOrEmpty(entity.Email))
        {
            return await context.AspNetUsers
              .FirstOrDefaultAsync(p => p.Email == entity.Email);
        }
        else if (!string.IsNullOrEmpty(entity.Id))
        {
            return await context.AspNetUsers
             .FirstOrDefaultAsync(p => p.Id == entity.Id);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Validate Email Address
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">User identifier container</param>
    /// <returns>The user record or null if not found</returns>
    public static async Task<bool> ValidateEmailAsync(
        ApplicationDBContext context,
        string Email)
    {
        var TotalRecords =  await context.AspNetUsers
            .CountAsync(p => p.Email == Email);

        if (TotalRecords > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Field Operations

    /// <summary>
    /// Updates a single field value for a user record
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="id">User ID</param>
    /// <param name="value">New field value</param>
    /// <param name="fieldName">Field name to update</param>
    public static async Task UpdateFieldAsync(
        ApplicationDBContext context,
        string id,
        dynamic value,
        string fieldName)
    {
        if (string.IsNullOrEmpty(id)) return;

        var user = await context.AspNetUsers
            .FirstOrDefaultAsync(p => p.Id == id);

        if (user == null) return;

        var property = user.GetType()
            .GetProperties()
            .FirstOrDefault(p =>
                string.Equals(p.Name, fieldName, StringComparison.OrdinalIgnoreCase));

        if (property != null)
        {
            property.SetValue(user, value);
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }

    #endregion

    #region Query Operations

    /// <summary>
    /// Loads users based on query parameters with caching support
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="query">Query parameters</param>
    /// <returns>List of matching users</returns>
    public static async Task<List<ApplicationUser>?> LoadItemsAsync(
        ApplicationDBContext context,
        UserQueryEntity query)
    {
        // Bypass cache if not enabled or duration is 0
        if (!query.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await FetchItemsAsync(context, query);
        }

        // Try to get from cache
        var cacheKey = GenerateCacheKey("lng_user", query);
        if (SiteConfigs.Cache != null && SiteConfigs.Cache.TryGetValue(cacheKey, out List<ApplicationUser>? cachedData))
        {
            return cachedData;
        }

        // Fetch fresh data and cache it
        var data = await FetchItemsAsync(context, query);
        if (SiteConfigs.Cache != null)
        {
            SiteConfigs.Cache.Set(
                cacheKey,
                data,
                new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(ApplicationSettings.Caching.Duration)));
        }

        return data;
    }

    /// <summary>
    /// Counts users matching query parameters with caching support
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="query">Query parameters</param>
    /// <returns>Count of matching users</returns>
    public static async Task<int> CountItemsAsync(
        ApplicationDBContext context,
        UserQueryEntity query)
    {
        // Bypass cache if not enabled or duration is 0
        if (!query.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await CountAsync(context, query);
        }

        // Try to get from cache
        var cacheKey = GenerateCacheKey("cnt_user", query);
        if (SiteConfigs.Cache != null && SiteConfigs.Cache.TryGetValue(cacheKey, out int cachedCount))
        {
            return cachedCount;
        }

        // Fetch fresh count and cache it
        var count = await CountAsync(context, query);
        if (SiteConfigs.Cache != null)
        {
            SiteConfigs.Cache.Set(
                cacheKey,
                count,
                new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(ApplicationSettings.Caching.Duration)));
        }

        return count;
    }

    #endregion

    #region Core Query Implementation

    private static async Task<List<ApplicationUser>> FetchItemsAsync(
        ApplicationDBContext context,
        UserQueryEntity query)
    {
        var queryable = ProcessConditions(PrepareQuery(context, query), query);
        return await LoadListAsync(queryable, query);
    }

    private static async Task<int> CountAsync(
        ApplicationDBContext context,
        UserQueryEntity query)
    {
        return await PrepareQuery(context, query).CountAsync();
    }

    private static async Task<List<ApplicationUser>> LoadListAsync(
        IQueryable<UserQuery> query,
        UserQueryEntity entity)
    {
        return await query.Select(PrepareList(entity)).ToListAsync();
    }

    private static Expression<Func<UserQuery, ApplicationUser>> PrepareList(UserQueryEntity entity)
    {
        return p => PrepareListItem(p, entity);
    }

    private static ApplicationUser PrepareListItem(UserQuery queryResult, UserQueryEntity entity)
    {
        var user = queryResult.User ?? throw new ArgumentNullException(nameof(queryResult.User));

        var record = new ApplicationUser
        {
            Id = user.Id,
            FirstName = user.FirstName,
            UserRole = user.UserRole,
            Url = Urls.PreviewUrl(new UrlEntity
            {
                Slug = user.Slug,
                Directory = UserUrls.GetDirectoryUrl()
            })
        };

        // Include additional fields for list/profile views or when specific IDs are requested
        if (entity.ColumnOptions == Types.FetchColumnOptions.List ||
            entity.ColumnOptions == Types.FetchColumnOptions.Profile ||
            !string.IsNullOrEmpty(entity.Id) ||
            !string.IsNullOrEmpty(entity.Slug))
        {
            record.IsEnabled = user.IsEnabled;
            record.IsFeatured = user.IsFeatured;
            record.PhoneNumber = user.PhoneNumber;
            record.Email = user.Email;
            record.CreatedAt = user.CreatedAt;
            record.EmailConfirmed = user.EmailConfirmed;
            record.UserName = user.UserName;
            record.Type = user.Type;
        }

        // Set avatar URL with fallback to default
        record.Avatar = UploadMedia.GetImageUrl(user.Avatar, UserSettings.DefaultThumb);

        // Include profile-specific fields if requested
        if (entity.ColumnOptions == Types.FetchColumnOptions.Profile ||
            !string.IsNullOrEmpty(entity.Id) ||
            !string.IsNullOrEmpty(entity.Slug))
        {
            // Additional profile-specific fields can be added here
        }

        return record;
    }

    public static IQueryable<UserQuery> ProcessConditions(IQueryable<UserQuery> collectionQuery, UserQueryEntity query)
    {
        if (!string.IsNullOrEmpty(query.Order))
            collectionQuery = (IQueryable<UserQuery>)collectionQuery.Sort(query.Order);

        if (string.IsNullOrEmpty(query.Id))
        {
            // skip logic
            if (query.PageNumber > 1)
                collectionQuery = collectionQuery.Skip(query.PageSize * (query.PageNumber - 1));
            // take logic
            if (!query.LoadAll)
                collectionQuery = collectionQuery.Take(query.PageSize);
        }

        return collectionQuery;
    }

    private static IQueryable<UserQuery> PrepareQuery(
        ApplicationDBContext context,
        UserQueryEntity entity)
    {
        if (entity.ActionType == UserEnum.ActionType.Management)
        {
            return context.AspNetUsers
                .Join(context.AspNetUserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                .Join(context.AspNetRoles,
                    ur => ur.userRole.RoleId,
                    role => role.Id,
                    (ur, role) => new UserQuery
                    {
                        User = ur.user,
                        Role = role
                    })
                .Where(ReturnWhereClause(entity));
        }

        return context.AspNetUsers
            .Select(user => new UserQuery { User = user })
            .Where(ReturnWhereClause(entity));
    }

    private static Expression<Func<UserQuery, bool>> ReturnWhereClause(UserQueryEntity entity)
    {
        var predicate = PredicateBuilder.New<UserQuery>(true);

        // ID-based lookup
        if (!string.IsNullOrEmpty(entity.Id))
        {
            predicate = predicate.And(p => p.User != null && p.User.Id == entity.Id);
        }
        // Slug-based lookup
        else if (!string.IsNullOrEmpty(entity.Slug))
        {
            predicate = predicate.And(p => p.User != null && p.User.Slug == entity.Slug);
        }
        // Slug prefix lookup
        else if (!string.IsNullOrEmpty(entity.SlugStartedWith))
        {
            predicate = predicate.And(p => p.User != null && p.User.Slug.StartsWith(entity.SlugStartedWith));
        }

        if (entity.AdvanceFilter)
        {
            // Role filtering
            if (!string.IsNullOrEmpty(entity.UserRoleId))
            {
                predicate = predicate.And(p => p.Role != null && p.Role.Id == entity.UserRoleId);
            }

            if (!string.IsNullOrEmpty(entity.UserRoleName))
            {
                predicate = predicate.And(p => p.Role != null && p.Role.Name == entity.UserRoleName);
            }

            // Status filtering
            if (entity.EmailConfirmed != Types.ActionTypes.All)
            {
                var emailConfirmed = entity.EmailConfirmed == Types.ActionTypes.Enabled;
                predicate = predicate.And(p => p.User != null && p.User.EmailConfirmed == emailConfirmed);
            }

            if (entity.IsEnabled != Types.ActionTypes.All)
            {
                predicate = predicate.And(p => p.User != null && p.User.IsEnabled == entity.IsEnabled);
            }

            if (entity.IsFeatured != Types.FeaturedTypes.All)
            {
                predicate = predicate.And(p => p.User != null && p.User.IsFeatured == entity.IsFeatured);
            }

            // Character-based filtering
            if (!string.IsNullOrEmpty(entity.Character))
            {
                predicate = predicate.And(p =>
                    p.User != null &&
                    !string.IsNullOrEmpty(p.User.FirstName) &&
                    p.User.FirstName.StartsWith(entity.Character));
            }

            // Date filtering
            if (entity.DateFilter != Types.DateFilter.All)
            {
                var now = UtilityHelper.TimeZoneOffsetDateTime();
                var startDate = new DateTime(now.Year, now.Month, 1);

                predicate = entity.DateFilter switch
                {
                    Types.DateFilter.Today =>
                        predicate.And(p => p.User != null && p.User.CreatedAt >= now.AddDays(-1)),
                    Types.DateFilter.ThisWeek =>
                        predicate.And(p => p.User != null && p.User.CreatedAt >= now.AddDays(-7)),
                    Types.DateFilter.ThisMonth =>
                        predicate.And(p => p.User != null && p.User.CreatedAt >= startDate.Date),
                    Types.DateFilter.PrevMonth =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt.Date >= startDate.AddMonths(-1) &&
                            p.User.CreatedAt.Date < startDate.Date),
                    Types.DateFilter.CurrentPrevMonth =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt.Date >= startDate.AddMonths(-1)),
                    Types.DateFilter.PrevThreeMonths =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt.Date >= startDate.AddMonths(-2)),
                    Types.DateFilter.PrevSixMonths =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt.Date >= startDate.AddMonths(-5)),
                    Types.DateFilter.ThisYear =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt.Year >= startDate.Year),
                    Types.DateFilter.ThisHour =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt >= now.AddHours(-1)),
                    Types.DateFilter.LastSixHour =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt >= now.AddHours(-6)),
                    Types.DateFilter.PrevYear =>
                        predicate.And(p => p.User != null &&
                            p.User.CreatedAt.Date >= new DateTime(now.Year - 1, 1, 1) &&
                            p.User.CreatedAt.Date < new DateTime(now.Year - 1, 12, 31)),
                    _ => predicate
                };
            }

            // Broad search
            if (!string.IsNullOrEmpty(entity.Term))
            {
                predicate = predicate.And(p => p.User != null &&
                    (p.User.FirstName.Contains(entity.Term) ||
                     p.User.LastName == entity.Term ||
                     p.User.Id == entity.Term));
            }
        }

        return predicate;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Processes bulk actions on users (enable, disable, delete, etc.)
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="users">List of users with action status</param>
    public static async Task ProcessApiActionsAsync(
        ApplicationDBContext context,
        List<ApplicationUser> users)
    {
        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.Id)) continue;

            switch (user.ActionStatus?.ToLower())
            {
                case "enable":
                    await ActivateUserAccount(context, user);
                    break;

                case "disable":
                    await UpdateFieldAsync(context, user.Id, (byte)Types.ActionTypes.Disabled, "IsEnabled");
                    break;

                case "delete":
                    await UpdateFieldAsync(context, user.Id, (byte)Types.ActionTypes.Archive, "IsEnabled");
                    break;

                case "permament_delete":
                    await DeleteAsync(context, user);
                    break;

                case "restore":
                    await UpdateFieldAsync(context, user.Id, (byte)Types.ActionTypes.Enabled, "IsEnabled");
                    break;
            }
        }
    }

    /// <summary>
    /// Generates a unique slug for a user
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="value">Base value to generate slug from</param>
    /// <param name="length">Maximum slug length</param>
    /// <param name="generateUniqueSlug">Whether to ensure uniqueness</param>
    /// <returns>Generated slug</returns>
    public static async Task<string> GenerateSlugAsync(
        ApplicationDBContext context,
        string? value,
        int length,
        bool generateUniqueSlug = false)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var slug = UtilityHelper.PrepareSlug(value, length);
        if (!generateUniqueSlug) return slug;

        // Check for existing slugs
        var existingCount = await CountAsync(context, new UserQueryEntity
        {
            Slug = slug,
            AdvanceFilter = false
        });

        if (existingCount == 0) return slug;

        // Handle conflicts by appending count
        var totalOccurrences = await CountAsync(context, new UserQueryEntity
        {
            SlugStartedWith = slug,
            AdvanceFilter = false
        });

        var uniqueSlug = $"{slug}-{totalOccurrences + 1}";

        // Verify new slug is actually unique
        existingCount = await CountAsync(context, new UserQueryEntity
        {
            Slug = uniqueSlug,
            AdvanceFilter = false
        });

        return existingCount == 0 ? uniqueSlug : $"{uniqueSlug}-{DateTime.Now.Ticks.ToString()[..10]}";
    }

    private static string GenerateCacheKey(string prefix, UserQueryEntity entity)
    {
        var builder = new StringBuilder(prefix)
            .Append('_')
            .Append(entity.PageNumber)
            .Append(entity.LocationId)
            .Append(entity.PageSize)
            .Append(entity.CompanyId);

        if (!string.IsNullOrEmpty(entity.Order))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(entity.Order.ToLower()));
        }

        if (!string.IsNullOrEmpty(entity.Country))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(entity.Country.ToLower()));
        }

        if (!string.IsNullOrEmpty(entity.State))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(entity.State.ToLower()));
        }

        if (!string.IsNullOrEmpty(entity.City))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(entity.City.ToLower()));
        }

        return builder.ToString();
    }

    #endregion
}

/// <summary>
/// Represents a joined query result for user data
/// </summary>
/// <remarks>
/// Combines user information with optional role details.
/// Used in complex queries that join multiple tables.
/// </remarks>
public class UserQuery
{
    public ApplicationUser? User { get; set; }
    public ApplicationRole? Role { get; set; }
}
