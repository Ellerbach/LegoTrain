// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using LegoTrain.Models;
using LegoTrain.Services;

List<string> defaultConfig = new List<string>()
{
    "circuit.png",
    "signal-black.png",
    "signal-red.png",
    "signal-green.png",
    "switch-left-str.png",
    "switch-left-trn.png",
    "switch-right-str.png",
    "switch-right-trn.png",
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Check if we have all out elements in the config directory, if not, create them from the default
// For example, if you are running with docker and you mount the drive, by default, it will be empty
foreach (var conf in defaultConfig)
{
    if (!File.Exists(Path.Combine(builder.Environment.ContentRootPath, "config", conf)))
    {
        File.Copy(
            Path.Combine(builder.Environment.ContentRootPath, "configdefault", conf),
            Path.Combine(builder.Environment.ContentRootPath, "config", conf));
    }
}

AppConfiguration config = AppConfiguration.Load();
config.Discovery = new LegoDiscovery();
builder.Services.AddSingleton(config);

// Need to figure out how to properly do this
var dir = new DirectoryInfo(@"/var/keys/");
if (!dir.Exists)
{
    dir.Create();
}

builder.Services.AddDataProtection().PersistKeysToFileSystem(dir);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "config")),
    RequestPath = "/config"
});

app.UseAuthorization();

// app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
