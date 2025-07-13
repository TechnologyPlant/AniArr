using AniArr.Server.Entities;
using AniArr.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddCors();

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(i => i.ColorBehavior = LoggerColorBehavior.Disabled);
});

var logger = loggerFactory.CreateLogger<Program>();

builder.Services.AddHttpClient();

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
            SonarrConnectionDetails = request,
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

app.MapPut("/SonarrConfig", async ([FromServices] SonarrService sonarrService, [FromBody] SonarrConfig request) =>
{
    try
    {
        await sonarrService.SaveSonarrConfig(request);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.MapGet("/SonarrConfig", async ([FromServices] SonarrService sonarrService) =>
{
    try
    {
        var config = await sonarrService.GetSonarrConfig();
        return Results.Ok(config);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.MapFallbackToFile("/index.html");

app.Run();