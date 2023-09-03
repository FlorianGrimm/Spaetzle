using Brimborium.Spaetzle.Hubs;

using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddOpenTelemetryGrpcServices();
builder.Services.AddOpenTelemetryHttpProtobufServices();
builder.Services.AddSignalR().AddMessagePackProtocol(
    options => {
        options.SerializerOptions = MessagePack.MessagePackSerializerOptions.Standard
            .WithSecurity(MessagePack.MessagePackSecurity.UntrustedData);
    });

builder.Services.AddControllers();

builder.Services.AddSingleton<ISpaetzleHubSink, SpaetzleHubSink>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen((swaggerGenOptions) => {

    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Spaetzle API",
        Description = "Developer OpenTelemetry Monitor",
        //TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "Example Contact",
        //    Url = new Uri("https://example.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
    });

    // optimize this
    // is their any attributte???
    swaggerGenOptions.CustomSchemaIds(type => type.ToString());
});

var app = builder.Build();

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

/*
 builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});
https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio
 */

//app.UseAuthorization();

//app.MapRazorPages();

app.MapHub<SpaetzleHub>("/wsapi");
app.MapControllers();
app.MapSwagger();

app.MapFallbackToFile("{*path}", "/index.html");

//app.MapFallbackToFile("{path:regex(^(?!api).$)}", "/index.html");

//app.MapFallbackToPage("/{*path}", "/");

//app.MapFallback("{*path:nonfile}", () => {
//    return "OK";
//});
//app.MapFallback("{*path}", (HttpRequest request) =>
//{
//    app.re
//    return "OK";
//});

app.Run();
