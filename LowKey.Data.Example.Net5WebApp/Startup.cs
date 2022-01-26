using LowKey.Data.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

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
            var connBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                UserID = Configuration.GetValue<string>("SQL_USERNAME"),
                Password = Configuration.GetValue<string>("SQL_PASSWORD")
            };
            var database = Configuration.GetValue<string>("SQL_DATABASE");
            var server = Configuration.GetValue<string>("SQL_SERVER");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddLowKeyData(config =>
            {
                // SINGLE TENANT
                config.AddStore(new DataStore("sql", database), server).WithSqlServer(connBuilder);


                // MULTI-TENANT
                var tenantResolver = new QueryStringTenantResolver(new Tenant(server, server));
                config.AddStore(new DataStore("sql-multi-tenant", "master"), 
                    tenantResolverFactory: cancel => Task.FromResult((ITenantResolver)tenantResolver),
                    tenantIdResolverFactory: cancel => Task.FromResult((ITenantIdResolver)tenantResolver))
                    .WithSqlServer(connBuilder);
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
