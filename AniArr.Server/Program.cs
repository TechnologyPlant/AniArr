using AniArr.Server.Endpoints;
using AniArr.Server.Entities;
using AniArr.Server.Services;
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

var anilistConfigGroup = app.MapGroup("/AnilistConfig");
anilistConfigGroup.MapAnilistConfigGroup();

var fribbListGroup = app.MapGroup("/FribbList");
fribbListGroup.MapFribbListGroup();

var sonarrConfigGroup = app.MapGroup("/Sonarr");
sonarrConfigGroup.MapSonarrGroup();

var watchlistGroup = app.MapGroup("/WatchListItem");
watchlistGroup.MapWatchlistGroup();

app.MapFallbackToFile("/index.html");

app.Run();