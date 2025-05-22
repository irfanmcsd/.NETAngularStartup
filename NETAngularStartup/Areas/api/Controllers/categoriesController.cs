using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NETAngularApp.Areas.api.Controllers
{
    /// <summary>
    /// API controller for managing categories and their related operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initializes a new instance of the CategoriesController
        /// </summary>
        /// <param name="context">Database context for category operations</param>
        public CategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Loads categories based on query parameters with optional pagination
        /// </summary>
        /// <returns>
        /// Returns either:
        /// - Success with category list and record count
        /// - Report placeholder message
        /// - Error response if processing fails
        /// </returns>
        [HttpPost("load")]
        public async Task<ActionResult> LoadCategories(CategoryQueryEntity entity)
        {
            try
            {
                if (entity == null)
                    return BadRequest(new { status = "error", message = "Invalid input data" });

                // Handle report generation request
                if (UtilityHelper.IsSelected(entity.RenderReport))
                    return Ok(new { status = "success", message = "Report integration later" });

                List<Categories>? posts;
                int? records = 0;

                if (entity.skip_record_stats)
                {
                    // Load data without pagination stats
                    posts = await CategoriesBLL.LoadItems(_context, entity);
                }
                else
                {
                    // Load data with pagination support
                    records = await CategoriesBLL.CountItems(_context, entity);
                    posts = records > 0 ? await CategoriesBLL.LoadItems(_context, entity) : new List<Categories>();
                }

                return Ok(new { status = "success", posts, records });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("LoadCategories", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error processing request. Reference: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Retrieves a single category by ID or slug
        /// </summary>
        /// <returns>
        /// Returns either:
        /// - Success with the requested category
        /// - Error if record not found or input invalid
        /// </returns>
        [HttpPost("getinfo")]
        public async Task<ActionResult> GetCategoryInfo(CategoryQueryEntity entity)
        {
            try
            {
                if (entity == null || (entity.Id <= 0 && string.IsNullOrEmpty(entity.Slug)))
                    return BadRequest(new { status = "error", message = "Invalid input parameters" });

                var categories = await CategoriesBLL.LoadItems(_context, entity);

                if (categories != null && categories.Count > 0)
                {
                    categories[0].CultureCategories = await CategoryDataBLL.FetchCultures(_context, categories[0].Id);
                }

                return categories != null && categories.Count > 0
                    ? Ok(new { status = "success", record = categories[0] })
                    : NotFound(new { status = "error", message = "No record found" });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("GetCategoryInfo", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error retrieving category. Reference: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Processes category creation or updates
        /// </summary>
        /// <returns>
        /// Returns either:
        /// - Success with the processed category record
        /// - Error if processing fails
        /// </returns>
        [HttpPost("proc")]
        public async Task<ActionResult> ProcessCategory(Categories entity)
        {
            try
            {
                // var json = await new StreamReader(Request.Body).ReadToEndAsync();
                //var entity = JsonConvert.DeserializeObject<Categories>(json);

                if (entity == null)
                    return BadRequest(new { status = "error", message = "Invalid category data" });

                var result = entity.Id == 0
                    ? await CategoriesBLL.Add(_context, entity)
                    : await CategoriesBLL.Update(_context, entity);

                return Ok(new { status = "success", record = result });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessCategory", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error processing category. Reference: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Validates and generates slugs for different entity types
        /// </summary>
        /// <returns>
        /// Returns either:
        /// - Success with generated slug
        /// - Error if input invalid
        /// </returns>
        [HttpPost("labelvalidator")]
        public async Task<ActionResult> ValidateLabel()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var entity = JsonConvert.DeserializeObject<Categories>(json);
                if (entity == null || string.IsNullOrEmpty(entity.Term))
                    return BadRequest(new { status = "error", message = "Invalid input data" });

                string slug = entity.Term!;
                slug = entity.Type switch
                {
                    CategoryEnum.Types.Categories => await CategoriesBLL.GenerateSlug(_context, slug, 60, true),  // Categories
                    CategoryEnum.Types.Users => await UsersBLL.GenerateSlugAsync(_context, slug, 60, true), // Users
                    CategoryEnum.Types.Blogs => await BlogsBLL.GenerateSlug(_context, slug, 60, true),      // Blog posts
                    _ => slug
                };

                return Ok(new { status = "success", slug });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ValidateLabel", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error validating label. Reference: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Processes batch actions on multiple categories
        /// </summary>
        /// <returns>
        /// Returns either:
        /// - Success if actions processed
        /// - Error if processing fails
        /// </returns>
        [HttpPost("action")]
        public async Task<ActionResult> ProcessBatchActions(List<Categories> actionList)
        {
            try
            {
                if (actionList == null)
                    return BadRequest(new { status = "error", message = "Invalid action list" });

                await CategoriesBLL.ProcessApiActions(_context, actionList);
                return Ok(new { status = "success", message = "Actions processed successfully" });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessBatchActions", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error processing batch actions. Reference: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Helper method to log errors consistently
        /// </summary>
        private async Task<ErrorLogs> LogErrorAsync(string methodName, Exception ex)
        {
            return await ErrorLogsBLL.Add(_context, new ErrorLogs
            {
                Description = $"Error in {methodName}",
                Url = $"CategoriesController -> {methodName}()",
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message
            });
        }
    }
}