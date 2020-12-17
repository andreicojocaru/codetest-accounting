using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CodeTest.Accounting.BFF
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    // note: we use launchSettings.json to set the URLs for the LOCAL environment
                    // for deployed environments, the ASPNETCORE_URLS environment variable will specify the URLs
                });
    }
}
