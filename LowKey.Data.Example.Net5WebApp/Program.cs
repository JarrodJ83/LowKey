using LowKey.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LowKey.Data.Example.Net5WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var connBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
                    connBuilder.UserID = "devpowertrack";
                    connBuilder.Password = "devpass";

                    services.AddLowKeyData(new CoreDb()).WithSqlServer(connBuilder);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
