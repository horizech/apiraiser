using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



namespace Apiraiser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            // var config = new ConfigurationBuilder()
            //         .SetBasePath(Directory.GetCurrentDirectory())
            //         .AddJsonFile("appsettings.json", optional: false)
            //         .AddCommandLine(args)
            //         .Build();

            // return Host.CreateDefaultBuilder(args)
            //     .ConfigureWebHostDefaults(webBuilder =>
            //     {
            //         webBuilder.UseStartup<Startup>();
            //         // webBuilder.UseUrls(config.GetSection("ConnectionStrings:HostUrls").Get<string[]>())
            //         // webBuilder.UseKestrel(options =>
            //         //     {
            //         //         options.Limits.MaxRequestBodySize = null;
            //         //     }
            //         // )
            //         // .UseSetting("https_port", "5001");
            //     });


            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // webBuilder.UseKestrel(options =>
                    //  {
                    //      options.Limits.MaxRequestBodySize = null; // or a given limit
                    //      options.Limits.MaxConcurrentConnections = 100;
                    //      options.Limits.MaxConcurrentUpgradedConnections = 100;
                    //      options.Limits.MaxRequestBodySize = 10 * 1024;
                    //      options.Limits.MinRequestBodyDataRate =
                    //          new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(bytesPerSecond: 100,
                    //              gracePeriod: TimeSpan.FromSeconds(10));
                    //      options.Limits.MinResponseDataRate =
                    //          new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(bytesPerSecond: 100,
                    //              gracePeriod: TimeSpan.FromSeconds(10));
                    //      options.ListenLocalhost(5000); // Listen(System.Net.IPAddress.Any, 5000);
                    //      options.ListenLocalhost(5001,
                    //          listenOptions =>
                    //          {
                    //              listenOptions.UseHttps("Certificates/localhost.pfx",
                    //                  "password");
                    //          });
                    //      // serverOptions.Listen(System.Net.IPAddress.Any, 5001,
                    //      //     listenOptions =>
                    //      //     {
                    //      //         listenOptions.UseHttps("Certificates/localhost.pfx",
                    //      //             "password");
                    //      //     });
                    //      options.Limits.KeepAliveTimeout =
                    //          TimeSpan.FromMinutes(2);
                    //      options.Limits.RequestHeadersTimeout =
                    //          TimeSpan.FromMinutes(1);
                    //  });

                    // webBuilder.ConfigureKestrel(serverOptions =>
                    // {
                    //     serverOptions.Limits.MaxConcurrentConnections = 100;
                    //     serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                    //     serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                    //     serverOptions.Limits.MinRequestBodyDataRate =
                    //         new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(bytesPerSecond: 100,
                    //             gracePeriod: TimeSpan.FromSeconds(10));
                    //     serverOptions.Limits.MinResponseDataRate =
                    //         new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(bytesPerSecond: 100,
                    //             gracePeriod: TimeSpan.FromSeconds(10));
                    //     serverOptions.ListenLocalhost(5000); // Listen(System.Net.IPAddress.Any, 5000);
                    //     serverOptions.ListenLocalhost(5001,
                    //         listenOptions =>
                    //         {
                    //             listenOptions.UseHttps("Certificates/localhost.pfx",
                    //                 "password");
                    //         });
                    //     // serverOptions.Listen(System.Net.IPAddress.Any, 5001,
                    //     //     listenOptions =>
                    //     //     {
                    //     //         listenOptions.UseHttps("Certificates/localhost.pfx",
                    //     //             "password");
                    //     //     });
                    //     serverOptions.Limits.KeepAliveTimeout =
                    //         TimeSpan.FromMinutes(2);
                    //     serverOptions.Limits.RequestHeadersTimeout =
                    //         TimeSpan.FromMinutes(1);
                    // });
                    // webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001");
                }
            );
        }
    }
}
