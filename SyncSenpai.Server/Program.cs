using SyncSenpai.Server.Components;
using Marten;
using Weasel.Core;

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
                // Establish the connection string to your Marten database
                options.Connection(builder.Configuration.GetConnectionString("Marten")!);

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
