using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationMessanger.Controllers
{
    public class MessageResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public MessageResponse(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }
    }
}
