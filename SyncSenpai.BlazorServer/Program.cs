using Blazored.Toast;
using Marten;
using SyncSenpai.Ani.Repositories;
using SyncSenpai.Ani.Services;
using SyncSenpai.BlazorServer.Components;
using SyncSenpai.Sonarr.Repositories;
using Weasel.Core;

namespace SyncSenpai.BlazorServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLogging();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddBlazoredToast();

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

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<AniService>();
            builder.Services.AddScoped<WatchListRepository>();
            builder.Services.AddScoped<SonarrConfigRepository>();

            builder.Services.AddScoped<ConfigRepository>();
            builder.Services.AddScoped<IAniService, AniService>();

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
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
