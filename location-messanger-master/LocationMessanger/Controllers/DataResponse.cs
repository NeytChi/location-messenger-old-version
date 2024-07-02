using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationMessanger.Controllers
{
    public class DataResponse
    {
        public bool success { get; set; }
        public dynamic data { get; set; }
        public DataResponse(bool Success,dynamic Data )
        {
            success = Success;
            data = Data;
        }
    }
}
