using LowKey.Data.Model;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddLowKeyData(config =>
{
    var sqlServer = builder.Configuration.GetValue<string>("SQL_SERVER");
    config
        .AddStore(new DataStore("sql_master", "master"), sqlServer)
        .WithSqlServer(new SqlConnectionStringBuilder
        {
            UserID = builder.Configuration.GetValue<string>("SQL_USERNAME"),
            Password = builder.Configuration.GetValue<string>("SQL_PASSWORD")
        });

    var postgresServer = builder.Configuration.GetValue<string>("NPGSQL_SERVER");
    config
        .AddStore(new DataStore("pg_main", "main"), postgresServer)
        .WithPostgres(new NpgsqlConnectionStringBuilder
        {
            Username = builder.Configuration.GetValue<string>("SQL_USERNAME"),
            Password = builder.Configuration.GetValue<string>("SQL_PASSWORD")
        });
});

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
