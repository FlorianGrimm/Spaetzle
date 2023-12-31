using Brimborium.Spaetzle.Contracts;
using Brimborium.Spaetzle.Interact;

namespace Brimborium.Spaetzle;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddSpaetzle(
        this WebApplicationBuilder builder
        )
    {
        // Add services to the container.
        //builder.Services.AddRazorPages();
        builder.Services.AddOpenTelemetryGrpcServices();
        builder.Services.AddOpenTelemetryHttpProtobufServices();
        builder.Services.AddSingleton<IRuleEngine>((services) => RuleEngine.Create(services));

        builder.Services.AddSignalR();
        //builder.Services.AddSignalR().AddMessagePackProtocol(
        //    options =>
        //    {
        //        options.SerializerOptions = MessagePack.MessagePackSerializerOptions.Standard
        //            .WithSecurity(MessagePack.MessagePackSecurity.UntrustedData);
        //    });

        builder.Services.AddControllers();

        builder.Services.AddSingleton<ISpaetzleHubSink, SpaetzleHubSink>();

        builder.Services.AddEndpointsApiExplorer();


        /*
        https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio
         */
        builder.Services.AddSwaggerGen((swaggerGenOptions) =>
        {

            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Spaetzle API",
                Description = "Developer OpenTelemetry Monitor",
            });

            // optimize this
            // is their any attributte???
            swaggerGenOptions.CustomSchemaIds(type =>
            {
                if (type.DeclaringType is null)
                {
                    return type.Name;
                } else
                {
                    return $"{type.DeclaringType.Name}-{type.Name}";
                }
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddSpaetzleRules<T>(
        this WebApplicationBuilder builder
    )
    where T : class, IRulesCollection
    {
        builder.Services.AddTransient<IRulesCollection, T>();
        return builder;
    }

    public static WebApplication UseSpaetzle(
        this WebApplication app
        )
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI(c => { });

        //app.UseAuthentication();
        //app.UseAuthorization();

        //app.MapRazorPages();

        app.MapHub<SpaetzleHub>("/wsapi");
        app.MapControllers();
        app.MapSwagger();

        app.MapFallbackToFile("{*path}", "/index.html");

        return app;
    }
}

