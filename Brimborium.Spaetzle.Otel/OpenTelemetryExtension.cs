namespace Brimborium.Spaetzle.Otel;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddOpenTelemetryGrpcServices(this IServiceCollection services) {
        services.AddSingleton<ICharonService, CharonService>();
        services.AddHostedService<BackgroundGPRCService>();
        return services;
    }
    public static IServiceCollection AddOpenTelemetryHttpProtobufServices(this IServiceCollection services)
    {
        services.AddSingleton<ICharonService, CharonService>();
        services.AddHostedService<BackgroundHttpProtobufService>();
        return services;
    }
}

