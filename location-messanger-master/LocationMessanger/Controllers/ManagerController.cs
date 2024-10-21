using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LocationMessanger.Responses;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;
using Serilog;

namespace LocationMessanger.Controllers
{
    [ApiController]
    [Route("v1.0/[controller]/[action]/")]
    public class ManagerController : ControllerBase
    {
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
                Log.Logger.Information("Return state urls, IP -> " 
                + HttpContext.Connection.RemoteIpAddress.ToString());
            return Ok(new DataResponse(result, new {url = result ? sender.UrlRedirect : "" }));
        }
    }
}