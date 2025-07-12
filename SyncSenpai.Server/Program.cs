using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using SyncSenpai.Ani.Repositories;
using SyncSenpai.Server.Entities;
using SyncSenpai.Server.Services;
using SyncSenpai.Sonarr.Repositories;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(i => i.ColorBehavior = LoggerColorBehavior.Disabled);
});

var logger = loggerFactory.CreateLogger<Program>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<SonarrConfigRepository>();
builder.Services.AddScoped<ConfigRepository>();
builder.Services.AddScoped<WatchListRepository>();

builder.Services.AddScoped<AniService>();
builder.Services.AddScoped<SonarrService>();
builder.Services.AddScoped<MongoDbService>();

var app = builder.Build();

app.UseDefaultFiles();

app.MapStaticAssets();

app.UseRouting();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

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

//app.MapGet("/watchlist", async ([FromServices] AniService aniService) =>
//{
//    return Results.Ok(await aniService.GetPendingEntriesAsync());
//})
//.WithName("GetWatchList");

app.MapGet("/userwatchlist/{userName}", async ([FromRoute] string userName, [FromServices] AniService aniService) =>
{
    return Results.Ok(await aniService.GetUserWatchListAsync(userName));
})
    .WithName("GetUserWatchList");

app.MapGet("/userwatchlist", async ([FromServices] AniService aniService, [FromServices] MongoDbService mongoDbServicee) =>
{
    var config = await mongoDbServicee.GetConfigAsync();
    return Results.Ok(await aniService.GetUserWatchListAsync(config.UserName));
});

app.MapGet("/userwatchlistupdate", async ([FromServices] AniService aniService, [FromServices] MongoDbService mongoDbService) =>
{
    var config = await mongoDbService.GetConfigAsync();
    return Results.Ok(await mongoDbService.GetUpdatedWatchlistEntries());
});

app.MapPatch("/AnilistConfig", async ([FromBody] string username, [FromServices] MongoDbService mongoDbService) =>
{
    if (String.IsNullOrEmpty(username))
        return Results.BadRequest();

    await mongoDbService.SetAniListUserNameAsync(username);
    return Results.Ok();
});

app.MapGet("/AnilistConfig", async ([FromServices] MongoDbService mongoDbService) =>
{
    var config = await mongoDbService.GetConfigAsync();
    return Results.Ok(config);
});

app.MapPost("/FribbList", async ([FromServices] MongoDbService mongoDbService, [FromBody] HttpRequest request) =>
{
    try
    {
        var form = await request.ReadFormAsync();
        var file = form.Files["file"]; ;
        await mongoDbService.StoreFribbItems(file);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }

    return Results.Ok();
});

app.MapPost("/SonarrConfig/test", async ([FromServices] SonarrService sonarrService, [FromBody] SonarrConnectionDetails request) =>
{
    try
    {
        var result = await sonarrService.TestConnection(request);
        if (!result) return Results.BadRequest();

        SonarrConfig config = new()
        {
            ConnectionDetails = request,
            SonarrTags = await sonarrService.GetSonarrTags(request),
            QualityProfiles = await sonarrService.LoadQualityProfiles(request),
            RootFolders = await sonarrService.LoadRootFolders(request)
        };

        return Results.Ok(config);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.MapFallbackToFile("/index.html");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
