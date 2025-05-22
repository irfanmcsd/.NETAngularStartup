using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NETAngularApp.Areas.api.Controllers
{
    /// <summary>
    /// API controller for managing tag operations including listing and bulk actions.
    /// Requires Bearer token authentication for all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initializes a new instance of the TagsController
        /// </summary>
        /// <param name="context">Database context for tag operations</param>
        public TagsController(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves tags based on query parameters with optional pagination
        /// </summary>
        /// <returns>
        /// Returns list of tags with total count or report placeholder
        /// </returns>
        /// <response code="200">Returns tag list or report placeholder</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="500">If an error occurs</response>
        [HttpPost("load")]
        public async Task<ActionResult> Load()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var query = JsonConvert.DeserializeObject<TagQueryEntity>(json);

                if (query == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid request format" });
                }

                // Handle report generation request
                if (UtilityHelper.IsSelected(query.RenderReport))
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "Report functionality will be implemented in a future version"
                    });
                }

                // Get data based on query parameters
                var (tags, count) = await GetTagData(query);
                return Ok(new
                {
                    status = "success",
                    posts = tags,
                    records = count
                });
            }
            catch (Exception ex)
            {
                await LogError("Load", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while retrieving tags"
                });
            }
        }

        /// <summary>
        /// Performs batch actions on multiple tags (enable/disable/delete etc.)
        /// </summary>
        /// <returns>Returns success status after processing actions</returns>
        /// <response code="200">Actions processed successfully</response>
        /// <response code="400">If the action list is invalid</response>
        /// <response code="500">If an error occurs</response>
        [HttpPost("action")]
        public async Task<ActionResult> ProcessActions(List<Tags> tags)
        {
            try
            {
                //var json = await new StreamReader(Request.Body).ReadToEndAsync();
                // var tags = JsonConvert.DeserializeObject<List<Tags>>(json);

                if (tags == null || tags.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "No valid tag actions provided"
                    });
                }

                await TagsBLL.ProcessApiActions(_context, tags);
                return Ok(new
                {
                    status = "success",
                    message = "Tag actions processed successfully"
                });
            }
            catch (Exception ex)
            {
                await LogError("ProcessActions", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while processing tag actions"
                });
            }
        }

        #region Private Helper Methods

        private async Task<(List<Tags> Tags, int? Count)> GetTagData(TagQueryEntity query)
        {
            if (query.skip_record_stats)
            {
                var tags = await TagsBLL.LoadItems(_context, query);
                return (tags, null);
            }

            var count = await TagsBLL.CountItems(_context, query);
            var result = count > 0
                ? await TagsBLL.LoadItems(_context, query)
                : new List<Tags>();

            return (result, count);
        }

        private async Task LogError(string method, Exception ex)
        {
            await ErrorLogsBLL.Add(_context, new ErrorLogs()
            {
                Description = $"Error in {nameof(TagsController)}.{method}",
                Url = $"{nameof(TagsController)} -> {method}()",
                StackTrace = ex.ToString()
            });
        }

        #endregion
    }
}