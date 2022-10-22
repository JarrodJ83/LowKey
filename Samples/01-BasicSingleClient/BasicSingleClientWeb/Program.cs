using LowKey.Data.Model;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var sqlServer = builder.Configuration.GetValue<string>("SQL_SERVER");
builder.Services.AddLowKeyData(config =>
{
    config
        .AddStore(new DataStore("sql_master", "master"), sqlServer)
        .WithSqlServer(new SqlConnectionStringBuilder
        {
            UserID = builder.Configuration.GetValue<string>("SQL_USERNAME"),
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
