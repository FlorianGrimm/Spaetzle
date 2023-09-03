namespace Brimborium.Spaetzle.Otel;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddOpenTelemetryGrpcServices(this IServiceCollection services) {
        services.AddSingleton<ICharonService, CharonService>();
        services.AddHostedService<BackgroundGPRCService>();
        services.AddHostedService<CharonToSinkLogService>();
        return services;
    }
    public static IServiceCollection AddOpenTelemetryHttpProtobufServices(this IServiceCollection services)
    {
        services.AddSingleton<ICharonService, CharonService>();
        services.AddHostedService<BackgroundHttpProtobufService>();
        services.AddHostedService<CharonToSinkLogService>();
        return services;
    }
}

