namespace Brimborium.Spaetzle.Otel.Services;

public class CharonToSinkMetricService : BackgroundService
{
    private readonly ICharonService _CharonService;
    private readonly IRuleEngine _RuleEngine;
    private readonly ISpaetzleHubSink _Sink;

    public CharonToSinkMetricService(
        ICharonService charonService,
        IRuleEngine ruleEngine,
        ISpaetzleHubSink sink
        )
    {
        this._CharonService = charonService;
        this._RuleEngine = ruleEngine;
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
                            this._RuleEngine.EnrichMetric(new OneMetric(itemMetric.Resource, itemScopeMetric.Scope, itemMetricRecord));
                        }
                    }
                    // TODO this._Sink.SendTraceMetric(itemMetric);
                    await this._Sink.SendDisplayMessage(itemMetric.ToString());
                    
                    continue;
                }
            }
            await readerMetrics.WaitToReadAsync(stoppingToken);
        }
    }
}