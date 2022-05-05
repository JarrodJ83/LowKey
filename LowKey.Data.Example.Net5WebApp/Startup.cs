using LowKey.Data.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

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
            var sqlConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                UserID = Configuration.GetValue<string>("SQL_USERNAME"),
                Password = Configuration.GetValue<string>("SQL_PASSWORD")
            };
            var sqlDatabase = Configuration.GetValue<string>("SQL_DATABASE");
            var sqlServer = Configuration.GetValue<string>("SQL_SERVER");

            var postgresConnBuilder = new NpgsqlConnectionStringBuilder
            {
                Username = Configuration.GetValue<string>("SQL_USERNAME"),
                Password = Configuration.GetValue<string>("SQL_PASSWORD")
            };

            var postgresDatabase = Configuration.GetValue<string>("NPGSQL_DATABASE");
            var postgresServer = Configuration.GetValue<string>("NPGSQL_SERVER");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddLowKeyData(config =>
            {
                // SQL // 

                // SINGLE TENANT
                config.AddStore(new DataStore(DataStores.Sql, sqlDatabase), sqlServer).WithSqlServer(sqlConnBuilder);


                // MULTI-TENANT
                var tenantResolver = new QueryStringTenantResolver(new Tenant(sqlServer, sqlServer));
                config.AddStore(new DataStore(DataStores.SqlMultiTenant, sqlDatabase), 
                    tenantResolverFactory: () => tenantResolver,
                    tenantIdResolverFactory: () => tenantResolver).WithSqlServer(sqlConnBuilder);



                // POSTGRES //

                // SINGLE TENANT
                config.AddStore(new DataStore(DataStores.Postgres, postgresDatabase), postgresServer).WithPostgres(postgresConnBuilder);


                // MULTI-TENANT
                config.AddStore(new DataStore(DataStores.PostgresMultiTenant, postgresDatabase),
                    tenantResolverFactory: () => tenantResolver,
                    tenantIdResolverFactory: () => tenantResolver).WithPostgres(postgresConnBuilder);
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
