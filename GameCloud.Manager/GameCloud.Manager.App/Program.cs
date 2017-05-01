using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GameCloud.Manager.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddJsonFile("hosting.json", optional: true)
                    .Build();
                var host = new WebHostBuilder()
                    .UseConfiguration(config)
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
