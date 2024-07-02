﻿using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LocationMessanger.Controllers
{
    public class HttpRequestSender
    {
        public string UrlRedirect = "";
        public string UrlCheck = "";
        public HttpRequestSender()
        {
            Config config = new();
            UrlRedirect = config.urlRedirect;
            UrlCheck = config.urlCheck;
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
                WebClient client = new();
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
