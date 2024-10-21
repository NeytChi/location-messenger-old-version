using LocationMessanger.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace LocationMessanger.Controllers
{
    public class HttpRequestSender
    {
        public string UrlRedirect = "";
        public string UrlCheck = "";
        public HttpRequestSender(IOptions<ServerSettings> settings)
        { 
            UrlRedirect = settings.Value.urlRedirect;
            UrlCheck = settings.Value.urlCheck;
        }
        public bool CheckUrlState()
        {
            string result = GetRequest(UrlCheck);
            if (result != null)
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(result);
                if (json.ContainsKey("success")
                && json["success"].Type == JTokenType.Boolean)
                {
                    
                    return json["success"].ToObject<bool>();
                }
            }
            return false;
        }
        public string GetRequest(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var client = new WebClient();
                client.Headers.Add("user-agent",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                Stream data = client.OpenRead(url);
                StreamReader reader = new(data);
                string result = reader.ReadToEnd();
                data.Close();
                reader.Close();
                
                return result;
            }
            return null;
        }
    }
}
