using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Rocky
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Host 
            CreateHostBuilder(args).Build().Run();
        }


        //  Host is the server !!
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();   // pass the builder a 'Startup' type  for configuration
                });
    }
}
