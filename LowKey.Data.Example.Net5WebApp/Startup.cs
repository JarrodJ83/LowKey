using LowKey.Data.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LowKey.Data.Example.Net5WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            connBuilder.UserID = Configuration.GetValue<string>("SQL_USERNAME");
            connBuilder.Password = Configuration.GetValue<string>("SQL_PASSWORD");
            var database = Configuration.GetValue<string>("SQL_DATABASE");
            var server = Configuration.GetValue<string>("SQL_SERVER");

            services.AddLowKeyData(config =>
            {
                var dataStore = new DataStoreId("master");
                config.AddStore(dataStore, server, database).WithSqlServer(connBuilder);
            });
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
