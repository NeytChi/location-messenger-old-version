using System;
using miniMessanger.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LocationMessanger
{
    public class Program
    {
        public static bool requestView = false;
        public static void Main(string[] args)
        {
           
            if (args != null)
            {
                if (args.Length >= 1)
                {
                    if (args[0] == "-c")
                    {
                        using (var context = new Context(true))
                        {
                            context.Database.EnsureDeleted();
                        }
                        Console.WriteLine("Database 'minimessanger' was deleted.");
                        return;
                    }
                }
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
