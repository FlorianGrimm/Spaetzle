namespace Brimborium.Spaetzle.Otel.Services;

public class CharonToSinkMetricService : BackgroundService
{
    private readonly ICharonService _CharonService;
    private readonly ISpaetzleHubSink _Sink;

    public CharonToSinkMetricService(
        ICharonService charonService,
        ISpaetzleHubSink sink
        )
    {
        this._CharonService = charonService;
        this._Sink = sink;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var readerMetrics = this._CharonService.ChannelMetrics.Reader;
        while (!stoppingToken.IsCancellationRequested)
        {
            {
                if (readerMetrics.TryRead(out var itemMetric))
                {
                    // itemMetric.Resource

                    foreach (var itemScopeMetric in itemMetric.ScopeMetrics)
                    {
                        // itemScopeMetric.Scope
                        foreach (var itemMetricRecord in itemScopeMetric.Metrics)
                        {
                            // TODO: filter / process
                            //this._Sink.AddLog(itemLogRecord.Body.StringValue);
                            await this._Sink.SendDisplayMessage(itemMetricRecord.ToString());
                        }
                    }
                    continue;
                }
            }
            await readerMetrics.WaitToReadAsync(stoppingToken);
        }
    }
}