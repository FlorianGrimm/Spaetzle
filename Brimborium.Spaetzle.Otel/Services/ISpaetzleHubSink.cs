using OpenTelemetry.Proto.Logs.V1;
using OpenTelemetry.Proto.Metrics.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace Brimborium.Spaetzle.Otel.Services;

public interface ISpaetzleHubSink
{
    Task SendDisplayMessage(string message);

    Task SendLog(ResourceLogs itemLog);
    Task SendTrace(ResourceSpans itemTrace);
    Task SendMetric(ResourceMetrics itemMetric);
}
