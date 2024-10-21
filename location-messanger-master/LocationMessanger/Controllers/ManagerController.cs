using Serilog;
using Serilog.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LocationMessanger.Responses;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;

namespace LocationMessanger.Controllers
{
    [ApiController]
    [Route("v1.0/[controller]/[action]/")]
    public class ManagerController : ControllerBase
    {
        public Logger log = new LoggerConfiguration()
            .WriteTo.File("./logs/log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        public HttpRequestSender sender;

        public ManagerController(IOptions<ServerSettings> settings)
        {
            sender = new HttpRequestSender(settings);
        }
       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult State()
        {
            bool result = sender.CheckUrlState();
                log.Information("Return state urls, IP -> " 
                + HttpContext.Connection.RemoteIpAddress.ToString());
            return Ok(new DataResponse(result, new {url = result ? sender.UrlRedirect : "" }));
        }
    }
}