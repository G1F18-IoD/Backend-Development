using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Server_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            //.UseUrls("http://192.168.0.19:5021") // This line should be used when getting connections from other computers. This has to be your own local IPv4
                .Build();
    }
}
