namespace Brimborium.Spaetzle.Otel.Services;

public class CharonToSinkTraceService : BackgroundService
{
    private readonly ICharonService _CharonService;
    private readonly IRuleEngine _RuleEngine;
    private readonly ISpaetzleHubSink _Sink;

    public CharonToSinkTraceService(
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
        var readerTraces = this._CharonService.ChannelTraces.Reader;
        while (!stoppingToken.IsCancellationRequested)
        {
            {
                if (readerTraces.TryRead(out var itemTrace))
                {
                    // itemTrace.Resource
                    foreach (var itemTraceScopeSpan in itemTrace.ScopeSpans)
                    {
                        // itemTraceScopeSpan.Scope
                        foreach (var itemTraceSpan in itemTraceScopeSpan.Spans)
                        {
                            this._RuleEngine.EnrichTraces(new OneTrace(itemTrace, itemTraceScopeSpan.Scope, itemTraceSpan));
                        }
                    }
                    // TODO this._Sink.SendTrace(itemTrace);
                    await this._Sink.SendDisplayMessage(itemTrace.ToString());
                    continue;
                }
            }

            await readerTraces.WaitToReadAsync(stoppingToken);
        }
    }
}
