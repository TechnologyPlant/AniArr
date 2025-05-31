using SyncSenpai.Server.Components;
using Marten;
using Weasel.Core;
using SyncSenpai.Ani.Services;

namespace SyncSenpai.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // This is the absolute, simplest way to integrate Marten into your
            // .NET application with Marten's default configuration
            builder.Services.AddMarten(options =>
            {
                var host = "localhost";
                var port = 5432;
                var db = Environment.GetEnvironmentVariable("POSTGRES_DB");
                var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
                var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

                var martenConnectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";

                var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("Postgres");

                Console.WriteLine(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

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
