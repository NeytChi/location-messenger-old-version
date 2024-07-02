using Serilog;
using System.IO;
using System.Net;
using Serilog.Core;
using Newtonsoft.Json;
using miniMessanger.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Common;

namespace LocationMessanger.Controllers
{
    [ApiController]
    [Route("v1.0/[controller]/[action]/")]
    public class ManagerController : ControllerBase
    {
        
        public Logger log = new LoggerConfiguration()
            .WriteTo.File("./logs/log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        public HttpRequestSender sender = new HttpRequestSender();

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