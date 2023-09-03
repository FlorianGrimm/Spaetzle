using OpenTelemetry.Proto.Logs.V1;
using OpenTelemetry.Proto.Metrics.V1;
using OpenTelemetry.Proto.Resource.V1;
using OpenTelemetry.Proto.Trace.V1;

using System.Threading.Channels;

namespace Brimborium.Spaetzle.Otel.Services;

public interface ICharonService
{
    Task AddLogs(DateTimeOffset utcNow, ExportLogsServiceRequest request, CancellationToken stopToken);

    Task AddMetrics(DateTimeOffset utcNow, ExportMetricsServiceRequest request, CancellationToken stopToken);

    Task AddTrace(DateTimeOffset utcNow, ExportTraceServiceRequest request, CancellationToken stopToken);

    Channel<ResourceLogs> ChannelLogs { get; }

    Channel<ResourceMetrics> ChannelMetrics { get; }

    Channel<ResourceSpans> ChannelTraces { get; }
}

public class CharonService : ICharonService
{

    public CharonService()
    {
        this.ChannelLogs = Channel.CreateBounded<global::OpenTelemetry.Proto.Logs.V1.ResourceLogs>(new BoundedChannelOptions(4000));
        this._WriterResourceLogs = this.ChannelLogs.Writer;

        this.ChannelMetrics = Channel.CreateBounded<global::OpenTelemetry.Proto.Metrics.V1.ResourceMetrics>(new BoundedChannelOptions(4000));
        this._WriterResourceMetrics = this.ChannelMetrics.Writer;

        this.ChannelTraces = Channel.CreateBounded<global::OpenTelemetry.Proto.Trace.V1.ResourceSpans>(new BoundedChannelOptions(4000));
        this._WriterTraces = this.ChannelTraces.Writer;
    }

    public Channel<ResourceLogs> ChannelLogs { get; }

    private readonly ChannelWriter<ResourceLogs> _WriterResourceLogs;

    public Channel<ResourceMetrics> ChannelMetrics { get; }

    private readonly ChannelWriter<ResourceMetrics> _WriterResourceMetrics;

    public Channel<ResourceSpans> ChannelTraces { get; }

    private readonly ChannelWriter<ResourceSpans> _WriterTraces;

    public async Task AddLogs(DateTimeOffset utcNow, ExportLogsServiceRequest request, CancellationToken stopToken)
    {
        foreach (var resourceLog in request.ResourceLogs) { 
            await this._WriterResourceLogs.WriteAsync(resourceLog, stopToken);
        }
    }

    public async Task AddMetrics(DateTimeOffset utcNow, ExportMetricsServiceRequest request, CancellationToken stopToken)
    {
        foreach (var resourceMetric in request.ResourceMetrics) { 
            await this._WriterResourceMetrics.WriteAsync(resourceMetric, stopToken);
        }
    }

    public async Task AddTrace(DateTimeOffset utcNow, ExportTraceServiceRequest request, CancellationToken stopToken)
    {
        foreach (var resourceSpan in request.ResourceSpans) {
            await this._WriterTraces.WriteAsync(resourceSpan, stopToken);
        }
    }
}
