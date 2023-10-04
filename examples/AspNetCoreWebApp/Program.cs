#define Authorization
using Brimborium.Spaetzle.Interact;

using Microsoft.AspNetCore.Authentication.Negotiate;

namespace Examples.AspNetCoreWebApp
{
    public class Program
    {
        private static bool UseAuthorization = false;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            UseAuthorization = string.Equals("true", builder.Configuration.GetSection("UseAuthorization").Value, StringComparison.OrdinalIgnoreCase);

            if (UseAuthorization)
            {
                // Add services to the container.
                builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                    .AddNegotiate();

                builder.Services.AddAuthorization(options =>
                {
                    // By default, all incoming requests will be authorized according to the default policy.
                    options.FallbackPolicy = options.DefaultPolicy;
                });
            }
            builder.Services.AddRazorPages();
            builder.AddOpenTelemetry();

            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            if (UseAuthorization)
            {
                app.UseAuthorization();
            }
            app.MapRazorPages();

            app.Run();
        }
    }
}
