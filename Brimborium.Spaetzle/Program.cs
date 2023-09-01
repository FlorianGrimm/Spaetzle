using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddOpenTelemetryGrpcServices();
builder.Services.AddOpenTelemetryHttpProtobufServices();

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

//app.UseAuthorization();

//app.MapRazorPages();

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
