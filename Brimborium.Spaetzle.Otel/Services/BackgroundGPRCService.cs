﻿using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using static System.Net.WebRequestMethods;

namespace Brimborium.Spaetzle.Otel.Services;
public class BackgroundGPRCService : BackgroundService
{
    private readonly IHostApplicationLifetime _HostApplicationLifetime;
    private readonly ICharonService _CharonService;
    private readonly ILogger<BackgroundGPRCService> _Logger;
    private Task _TaskExecute;

    public BackgroundGPRCService(
        IHostApplicationLifetime hostApplicationLifetime,
        ICharonService charonService,
        ILogger<BackgroundGPRCService> logger
        )
    {
        this._TaskExecute = Task.CompletedTask;
        this._HostApplicationLifetime = hostApplicationLifetime;
        this._CharonService = charonService;
        this._Logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._TaskExecute = this.GrpcMain(stoppingToken);
        return this._TaskExecute;
    }

    private async Task GrpcMain(CancellationToken stoppingToken)
    {
        using var ctsMerged = CancellationTokenSource.CreateLinkedTokenSource(
            stoppingToken,
            this._HostApplicationLifetime.ApplicationStopping);

        // var builder = WebApplication.CreateBuilder(new string[] { "--urls", "https://0.0.0.0:4317" });
        // var builder = WebApplication.CreateBuilder(new string[] { "--urls", "http://0.0.0.0:4317" });
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddJsonFile("appsettings.OTEL.json", optional: true, reloadOnChange: true);

        builder.Services.AddGrpc();
        builder.Services.AddSingleton<ICharonService>(this._CharonService);

        builder.WebHost.UseKestrel((kestrelServerOptions) =>
        {
            kestrelServerOptions.ListenAnyIP(4317, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        //builder.Services.AddHttpLogging((httpLoggingOptions) => {
        //    //httpLoggingOptions.RequestHeaders.Add()
        //});

        var app = builder.Build();
        //app.UseHttpLogging();
        app.MapGrpcService<LogsService>();
        app.MapGrpcService<MetricsService>();
        app.MapGrpcService<TraceService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
        app.MapFallback("{*path}", (HttpRequest request) =>
        {
            this._Logger.LogError("MapFallback Path:{Path}", request.Path);
            return "OK";
        });
        //app.MapFallback("{*path}", CreateProxyRequestDelegate(endpoints, new SpaOptions { SourcePath = sourcePath }, npmScript, port, https, runner, regex, forceKill, wsl));

        //await app.RunAsync(ctsMerged.Token);

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
}