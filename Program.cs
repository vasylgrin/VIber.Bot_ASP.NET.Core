using Viber.Bot;
using VIber.Bot_ASP.NET.Core.Middleware;
using VIber.Bot_ASP.NET.Core.Models;

namespace VIber.Bot_ASP.NET.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            new ViberBotClient(Constants.BOT_TOKEN)
                .SetWebhookAsync(Constants.WEBHOOK_URL);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            //Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseMiddleware<ViberWebhookMiddleware>();

            app.Run();
        }
    }
}