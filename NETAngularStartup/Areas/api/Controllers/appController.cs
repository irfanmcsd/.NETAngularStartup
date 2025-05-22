using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicTradeBot.Areas.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    //[AutoValidateAntiforgeryToken]
    public class appController : ControllerBase
    {
        ApplicationDBContext _context;
        public appController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("init")]
        public async Task<ActionResult> init()
        {
            try
            {
                return Ok(new {
                    status = "success",
                    settings = new
                    {
                        keys = AppSecrets.GetSettings(),
                        general = ApplicationSettings.GetSettings(),
                        media = MediaSettings.GetSettings(),
                        category = CategorySettings.GetSettings(),
                        blog = BlogSettings.GetSettings(),
                        user = UserSettings.GetSettings()
                    },
                    cultures = CultureUtil.SupportedCultureList(),
                    general = Types.GetSettings(),
                    blog = BlogEnum.getSettings(),
                    category = CategoryEnum.GetSettings(),
                    role = RoleEnum.getSettings(),
                    tag = TagEnum.getSettings(),
                    user = UserEnum.getSettings()
                });
            }
            catch (Exception ex)
            {
                // log error
                var errorObj = await ErrorLogsBLL.Add(_context, new ErrorLogs()
                {
                    Description = "Error - Initialize App",
                    Url = "initController -> init()",
                    StackTrace = ex.StackTrace
                });

                return Ok(new { status = "error", message = "Error occured while processing your request: ErrorID: " + errorObj.Id });
            }
        }
    }
}
