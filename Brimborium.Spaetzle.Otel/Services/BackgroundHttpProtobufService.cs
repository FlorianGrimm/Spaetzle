using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Brimborium.Spaetzle.Otel.Services;

//
public class BackgroundHttpProtobufService : BackgroundService
{
    private readonly IHostApplicationLifetime _HostApplicationLifetime;
    private readonly ICharonService _CharonService;
    private readonly ILogger<BackgroundHttpProtobufService> _Logger;
    private Task _TaskExecute;

    public BackgroundHttpProtobufService(
        IHostApplicationLifetime hostApplicationLifetime,
        ICharonService charonService,
        ILogger<BackgroundHttpProtobufService> logger
        )
    {
        this._TaskExecute = Task.CompletedTask;
        this._HostApplicationLifetime = hostApplicationLifetime;
        this._CharonService = charonService;
        this._Logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._TaskExecute = this.HttpProtobufMain(stoppingToken);
        return this._TaskExecute;
    }

    private async Task HttpProtobufMain(CancellationToken stoppingToken)
    {
        using var ctsMerged = CancellationTokenSource.CreateLinkedTokenSource(
            stoppingToken,
            this._HostApplicationLifetime.ApplicationStopping);

        // var builder = WebApplication.CreateBuilder(new string[] { "--urls", "http://localhost:4318" });
        // var builder = WebApplication.CreateBuilder(new string[] { "--urls", "http://0.0.0.0:4318" });
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSingleton<ICharonService>(this._CharonService);

        builder.WebHost.UseKestrel((kestrelServerOptions) => {
            kestrelServerOptions.ListenAnyIP(4318, listenOptions => {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });
        });

        var app = builder.Build();

        app.Map("/v1/logs", (HttpContext httpContext) => this.V1Logs(httpContext));
        app.Map("/v1/metrics", (HttpContext httpContext) => this.V1Metrics(httpContext));
        app.Map("/v1/traces", (HttpContext httpContext) => this.V1Traces(httpContext));

        app.MapGet("/", () => "HTTP OTEL client.");

        app.MapFallback("{*path}", (HttpRequest request) =>
        {
            this._Logger.LogError("MapFallback Path:{Path}", request.Path);
            return "OK";
        });

        var token = ctsMerged.Token;
        await app.StartAsync(token).ConfigureAwait(false);

        IHostApplicationLifetime thisApplicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        this._HostApplicationLifetime.ApplicationStopped.Register((thisApplicationLifetime) =>
        {
            ((IHostApplicationLifetime)thisApplicationLifetime!).StopApplication();
        }, thisApplicationLifetime);

        token.Register((thisApplicationLifetime) =>
        {
            ((IHostApplicationLifetime)thisApplicationLifetime!).StopApplication();
        },
        thisApplicationLifetime);

        var waitForStop = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        thisApplicationLifetime.ApplicationStopping.Register(waitForStop =>
        {
            var tcs = (TaskCompletionSource<object?>)waitForStop!;
            tcs.TrySetResult(null);
        }, waitForStop);

        await waitForStop.Task.ConfigureAwait(false);

        // Host will use its default ShutdownTimeout if none is specified.
        // The cancellation token may have been triggered to unblock waitForStop. Don't pass it here because that would trigger an abortive shutdown.
        await app.StopAsync(CancellationToken.None).ConfigureAwait(false);
    }

    private async Task<IResult> V1Logs(HttpContext httpContext)
    {
        try
        {
            using var memoryStream = new System.IO.MemoryStream();
            await httpContext.Request.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0L;

            var request = global::OpenTelemetry.Proto.Collector.Logs.V1.ExportLogsServiceRequest.Parser.ParseFrom(memoryStream);
            var utcNow = DateTimeOffset.UtcNow;
            await this._CharonService.AddLogs(utcNow, request);
        } catch
        {
        }

        return Results.Ok();
    }

    private async Task<IResult> V1Metrics(HttpContext httpContext)
    {
        try
        {
            using var memoryStream = new System.IO.MemoryStream();
            await httpContext.Request.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0L;

            var request = global::OpenTelemetry.Proto.Collector.Metrics.V1.ExportMetricsServiceRequest.Parser.ParseFrom(memoryStream);
            var utcNow = DateTimeOffset.UtcNow;
            await this._CharonService.AddMetrics(utcNow, request);
        } catch { }
        return Results.Ok();
    }

    private async Task<IResult> V1Traces(HttpContext httpContext)
    {
        try
        {
            using var memoryStream = new System.IO.MemoryStream();
            await httpContext.Request.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0L;

            var request = global::OpenTelemetry.Proto.Collector.Trace.V1.ExportTraceServiceRequest.Parser.ParseFrom(memoryStream);
            var utcNow = DateTimeOffset.UtcNow;
            await this._CharonService.AddTrace(utcNow, request);
        } catch
        {
        }
        return Results.Ok();
    }
}
