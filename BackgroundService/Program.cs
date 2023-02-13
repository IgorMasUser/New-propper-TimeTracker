using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using BackgroundService.Host.Extensions;

namespace BackgroundService.Host
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
            return 0;
        }
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder builder = new HostBuilder();
            return builder.ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransitServices(hostContext.Configuration);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.Configure(app =>
                    {
                        app.UseRouting();
                    });
                });
        }
    }
}
