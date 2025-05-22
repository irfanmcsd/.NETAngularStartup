using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace MagicTradeBot.Areas.api.Controllers
{
    /// <summary>
    /// API controller for managing error log operations including viewing, clearing, and bulk actions.
    /// Requires Bearer token authentication for all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LogController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initializes a new instance of the LogController
        /// </summary>
        /// <param name="context">Database context for log operations</param>
        public LogController(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves error logs based on query parameters with optional pagination
        /// </summary>
        /// <returns>
        /// Returns list of error logs with total count or report data based on query parameters
        /// </returns>
        [HttpPost("load")]
        public async Task<ActionResult> Load()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var query = JsonConvert.DeserializeObject<ErrorLogQueryEntity>(json);

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
                        message = "Report integration pending implementation"
                    });
                }

                // Get data based on query parameters
                var (logs, count) = await GetLogData(query);
                return Ok(new { status = "success", posts = logs, records = count });
            }
            catch (Exception ex)
            {
                await LogError("Load", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while retrieving logs"
                });
            }
        }

        /// <summary>
        /// Clears all error logs from the system
        /// </summary>
        /// <returns>Returns success status after clearing logs</returns>
        [HttpPost("deleteall")]
        public async Task<ActionResult> DeleteAll()
        {
            try
            {
                await ErrorLogsBLL.DeleteAll(_context);
                return Ok(new
                {
                    status = "success",
                    message = "All logs cleared successfully"
                });
            }
            catch (Exception ex)
            {
                await LogError("DeleteAll", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while clearing logs"
                });
            }
        }

        /// <summary>
        /// Performs batch actions on multiple error logs
        /// </summary>
        /// <returns>Returns success status after processing actions</returns>
        [HttpPost("action")]
        public async Task<ActionResult> ProcessActions(List<ErrorLogs> logs)
        {
            try
            {
                //var json = await new StreamReader(Request.Body).ReadToEndAsync();
                // var logs = JsonConvert.DeserializeObject<List<ErrorLogs>>(json);

                if (logs == null || logs.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "No valid log actions provided"
                    });
                }

                await ErrorLogsBLL.ProcessApiActions(_context, logs);
                return Ok(new
                {
                    status = "success",
                    message = "Log actions processed successfully"
                });
            }
            catch (Exception ex)
            {
                await LogError("ProcessActions", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "An error occurred while processing log actions"
                });
            }
        }

        #region Private Helper Methods

        private async Task<(List<ErrorLogs> Logs, int? Count)> GetLogData(ErrorLogQueryEntity query)
        {
            if (query.skip_record_stats)
            {
                var logs = await ErrorLogsBLL.LoadItems(_context, query);
                return (logs, null);
            }

            var count = await ErrorLogsBLL.CountItems(_context, query);
            var result = count > 0
                ? await ErrorLogsBLL.LoadItems(_context, query)
                : new List<ErrorLogs>();

            return (result, count);
        }

        private async Task LogError(string method, Exception ex)
        {
            await ErrorLogsBLL.Add(_context, new ErrorLogs()
            {
                Description = $"Error in {nameof(LogController)}.{method}",
                Url = $"{nameof(LogController)} -> {method}()",
                StackTrace = ex.ToString()
            });
        }

        #endregion
    }
}