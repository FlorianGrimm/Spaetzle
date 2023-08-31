namespace Brimborium.Spaetzle.Otel.Services;

public class MetricsService : global::OpenTelemetry.Proto.Collector.Metrics.V1.MetricsService.MetricsServiceBase {
    private readonly ICharonService _CharonService;

    public MetricsService(
        ICharonService charonService
        )
    {
        this._CharonService = charonService;
    }

    public override async Task<ExportMetricsServiceResponse> Export(ExportMetricsServiceRequest request, ServerCallContext context)
    {
        var utcNow = DateTimeOffset.UtcNow;
        await this._CharonService.AddMetrics(utcNow, request);
        return new ExportMetricsServiceResponse();
    }
}
