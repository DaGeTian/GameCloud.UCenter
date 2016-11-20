using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GameCloud.UCenter.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

#if DEBUG
            builder.UseUrls("http://localhost:5001");
#endif
            var host = builder.Build();
            host.Run();
        }
    }
}
