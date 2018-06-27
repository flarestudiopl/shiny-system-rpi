using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Autofac.Extensions.DependencyInjection;

namespace HeatingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                              .ConfigureServices(services => services.AddAutofac());

#if DEBUG
            host = host.UseUrls("http://*:8000");
#else
            host = host.UseUrls("http://*:80");
#endif

            return host.UseStartup<Startup>()
                       .Build();
        }
    }
}
