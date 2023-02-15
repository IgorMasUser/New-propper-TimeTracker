using System;
using System.Threading.Tasks;
using BackgroundService.Host.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

//namespace BS
//{
//    //public class Program
//    //{
//    //    public static async Task<int> Main(string[] args)
//    //    {
//    //        await CreateHostBuilder(args).Build().RunAsync();
//    //        return 0;
//    //    }
//    //    private static IHostBuilder CreateHostBuilder(string[] args)
//    //    {
//    //        IHostBuilder builder = new HostBuilder();
//    //        return builder.ConfigureServices((hostContext, services) =>
//    //        {
//    //            services.AddHostedService<MyMassTransitHostedService>();
//    //        })
//    //            .ConfigureWebHostDefaults(builder =>
//    //            {
//    //                builder.Configure(app =>
//    //                {
//    //                    app.UseRouting();
//    //                });
//    //            });
//    //    }
//    //}

    
//}

Host.CreateDefaultBuilder().ConfigureServices((context,services)=>
services.AddHostedService<MyMassTransitHostedService>())
.Build()
.Run();
