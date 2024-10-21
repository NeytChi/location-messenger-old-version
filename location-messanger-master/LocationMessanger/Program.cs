using System;
using miniMessanger.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace LocationMessanger
{
    public class Program
    {
        public static bool requestView = false;

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
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
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
