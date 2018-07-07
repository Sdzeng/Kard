using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;

namespace Kard.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            //正式环境启用
            var config = new ConfigurationBuilder()
           .AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true)
           .Build();

            NLogBuilder.ConfigureNLog("nlog.config");

            return WebHost.CreateDefaultBuilder(args)
                .UseNLog()
              .UseConfiguration(config)
                .UseStartup<Startup>()
                // .UseIISIntegration()
                .Build();
        }
    }
}
