using Marten;
using Microsoft.AspNetCore.Mvc;
using SyncSenpai.Ani.Repositories;
using SyncSenpai.Ani.Services;
using SyncSenpai.Sonarr.Repositories;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddCors();

// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("Postgres");

    if (string.IsNullOrEmpty(connString))
        throw new InvalidOperationException("No valid connection string found");


    // Establish the connection string to your Marten database
    options.Connection(connString);

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<SonarrConfigRepository>();
builder.Services.AddScoped<ConfigRepository>();
builder.Services.AddScoped<WatchListRepository>();

builder.Services.AddScoped<AniService>();

var app = builder.Build();

app.UseDefaultFiles();

app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/watchlist", async ([FromServices] AniService aniService) =>
{
    return Results.Ok(await aniService.GetPendingEntriesAsync());
})
.WithName("GetWatchList");

app.MapGet("/userwatchlist/{userName}", async ([FromRoute] string userName, [FromServices] AniService aniService) =>
{
    return Results.Ok(await aniService.GetUserWatchListAsync(userName));
})
    .WithName("GetUserWatchList");

app.MapGet("/userwatchlist", async ([FromServices] AniService aniService) =>
{
    var config = await aniService.GetConfigAsync();
    return Results.Ok(await aniService.GetUserWatchListAsync(config.UserName));
});

app.MapPatch("/AnilistConfig", async ([FromBody] string username, [FromServices] AniService aniService) =>
{
    if (String.IsNullOrEmpty(username))
        return Results.BadRequest();

    await aniService.SetAniListUserNameAsync(username);
    return Results.Ok();
});

app.MapGet("/AnilistConfig", async ([FromServices] AniService aniService) =>
{
    var config = await aniService.GetConfigAsync();
    return Results.Ok(config);
});

app.MapFallbackToFile("/index.html");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
