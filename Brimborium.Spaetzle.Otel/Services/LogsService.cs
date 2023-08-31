namespace Brimborium.Spaetzle.Otel.Services;

public class LogsService : global::OpenTelemetry.Proto.Collector.Logs.V1.LogsService.LogsServiceBase {
    private readonly ICharonService _CharonService;

    public LogsService(
        ICharonService charonService
        )
    {
        this._CharonService = charonService;
    }

    public override async Task<ExportLogsServiceResponse> Export(ExportLogsServiceRequest request, ServerCallContext context)
    {
        var utcNow = DateTimeOffset.UtcNow;
        await this._CharonService.AddLogs(utcNow, request);
        return new ExportLogsServiceResponse();
    }
}
