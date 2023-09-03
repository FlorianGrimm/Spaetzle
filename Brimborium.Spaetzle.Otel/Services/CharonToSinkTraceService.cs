namespace Brimborium.Spaetzle.Otel.Services;

public class CharonToSinkTraceService : BackgroundService
{
    private readonly ICharonService _CharonService;
    private readonly ISpaetzleHubSink _Sink;

    public CharonToSinkTraceService(
        ICharonService charonService,
        ISpaetzleHubSink sink
        )
    {
        this._CharonService = charonService;
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
                            // TODO: filter / process
                            //this._Sink.AddTrace(itemTraceSpan);
                            await this._Sink.SendDisplayMessage(itemTraceSpan.ToString());
                        }
                    }
                    continue;
                }
            }
           
            await readerTraces.WaitToReadAsync(stoppingToken);
        }
    }
}
