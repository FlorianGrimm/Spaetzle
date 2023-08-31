namespace Brimborium.Spaetzle.Otel.Services;

public class TraceService : global::OpenTelemetry.Proto.Collector.Trace.V1.TraceService.TraceServiceBase
{
    private readonly ICharonService _CharonService;

    public TraceService(
        ICharonService charonService
        )
    {
        this._CharonService = charonService;
    }

    public override async Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {
        var utcNow = DateTimeOffset.UtcNow;
        await this._CharonService.AddTrace(utcNow, request);
        return new ExportTraceServiceResponse();
    }
}
