using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicTradeBot.Areas.api.Controllers
{
    /// <summary>
    /// API controller for managing blog operations including CRUD, listing, and bulk actions.
    /// Requires Bearer token authentication for all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initializes a new instance of the BlogsController
        /// </summary>
        /// <param name="context">Database context for blog operations</param>
        public BlogsController(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Loads blog posts with optional filtering, pagination, and reporting
        /// </summary>
        /// <returns>
        /// Returns blog posts with total count or generated report based on query parameters
        /// </returns>
        [HttpPost("load")]
        public async Task<ActionResult> Load()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var query = JsonConvert.DeserializeObject<BlogQueryEntity>(json);

                if (query == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid request format" });
                }

                // Handle report generation request
                if (UtilityHelper.IsSelected(query.RenderReport))
                {
                    var report = await BlogReportBLL.Generate(_context, query);
                    return Ok(new { status = "success", posts = report });
                }

                // Get data based on query parameters
                var (posts, count) = await GetBlogData(query);
                return Ok(new { status = "success", posts, records = count });
            }
            catch (Exception ex)
            {
                await LogError("Load", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while processing your request"
                });
            }
        }

        /// <summary>
        /// Gets detailed information for a single blog post including associated categories
        /// </summary>
        /// <returns>Returns blog post details with category information</returns>
        [HttpPost("getinfo")]
        public async Task<ActionResult> GetInfo()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var query = JsonConvert.DeserializeObject<BlogQueryEntity>(json);

                if (query == null)
                    return BadRequest(new { status = "error", message = "Invalid information" });

                if (query?.Id <= 0)
                {
                    return BadRequest(new { status = "error", message = "Invalid blog ID" });
                }

                var posts = await BlogsBLL.LoadItems(_context, query);
                if (posts == null || posts.Count == 0)
                {
                    return NotFound(new { status = "error", message = "Blog post not found" });
                }

                
                var blog = posts[0];
                blog.BlogCultureData = await BlogDataBLL.FetchCultures(_context, blog.Id);
                blog.CategoryList = await CategoryContentsBLL.FetchContentCategoryList(
                      _context,
                      query.Id,
                      "en",
                      CategoryEnum.Types.Blogs);


                return Ok(new { status = "success", record = blog });
            }
            catch (Exception ex)
            {
                await LogError("GetInfo", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while fetching blog details"
                });
            }
        }

        /// <summary>
        /// Creates or updates a blog post
        /// </summary>
        /// <returns>Returns the created/updated blog post</returns>
        [HttpPost("proc")]
        public async Task<ActionResult> ProcessBlog(Blogs blog)
        {
            try
            {
                //var json = await new StreamReader(Request.Body).ReadToEndAsync();
                //var blog = JsonConvert.DeserializeObject<Blogs>(json);

                if (blog == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid blog data" });
                }

                var result = blog.Id == 0
                    ? await BlogsBLL.Add(_context, blog)
                    : await BlogsBLL.Update(_context, blog);

                // process tags
                if (!string.IsNullOrEmpty(blog.Tags))
                {
                    await TagsBLL.ProcessTags(_context, blog.Tags, TagEnum.Types.Blog, TagEnum.TagType.Normal);
                }    
                return Ok(new { status = "success", record = result });
            }
            catch (Exception ex)
            {
                await LogError("ProcessBlog", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while saving the blog"
                });
            }
        }

        /// <summary>
        /// Performs batch actions on multiple blog posts
        /// </summary>
        /// <returns>Returns success status after processing actions</returns>
        [HttpPost("action")]
        public async Task<ActionResult> ProcessActions(List<Blogs> blogs)
        {
            try
            {
                // var json = await new StreamReader(Request.Body).ReadToEndAsync();
                // var blogs = JsonConvert.DeserializeObject<List<Blogs>>(json);

                if (blogs == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid action list" });
                }

                await BlogsBLL.ProcessApiActions(_context, blogs);
                return Ok(new { status = "success", message = "Actions completed successfully" });
            }
            catch (Exception ex)
            {
                await LogError("ProcessActions", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while processing actions"
                });
            }
        }

        #region Private Helper Methods

        private async Task<(List<Blogs> Posts, int? Count)> GetBlogData(BlogQueryEntity query)
        {
            if (query.skip_record_stats)
            {
                var posts = await BlogsBLL.LoadItems(_context, query);
                return (posts, null);
            }

            var count = await BlogsBLL.CountItems(_context, query);
            var result = count > 0
                ? await BlogsBLL.LoadItems(_context, query)
                : new List<Blogs>();

            return (result, count);
        }

        private async Task LogError(string method, Exception ex)
        {
            await ErrorLogsBLL.Add(_context, new ErrorLogs()
            {
                Description = $"Error in {nameof(BlogsController)}.{method}",
                Url = $"{nameof(BlogsController)} -> {method}()",
                StackTrace = ex.ToString()
            });
        }

        #endregion
    }
}