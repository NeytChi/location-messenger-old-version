using System;
using Common;
using Serilog;
using System.IO;
using Serilog.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;

namespace miniMessanger
{
    public class FileSaver
    {
        public ILogger log = Log.Logger;
        public string savepath;
        public FileSaver(IOptions<ServerSettings> settings)
        {
            this.savepath = settings.Value.savePath;
            
        }
        public void DeleteFile(string relativePath)
        {
            if (File.Exists(savepath + relativePath))
            {
                File.Delete(savepath + relativePath);
                log.Information("Delete file, path ->" + relativePath);
            }
        }
        public string CreateFile(IFormFile file, string relativePath)
        {
            DateTime now = DateTime.Now;
            string dir = relativePath + now.Year + "-" + now.Month + "-" + now.Day;

            Directory.CreateDirectory(savepath + dir);
            
            string url = dir + "/" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            file.CopyTo(new FileStream(savepath + url, FileMode.Create));
            log.Information("Create new file, relative path ->" + url);
            return url;
        }
    }   
}