﻿namespace Brimborium.Spaetzle.Otel.Services;

public class CharonToSinkLogService : BackgroundService
{
    private readonly ICharonService _CharonService;
    private readonly IRuleEngine _RuleEngine;
    private readonly ISpaetzleHubSink _Sink;

    public CharonToSinkLogService(
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
        var readerLogs = this._CharonService.ChannelLogs.Reader;
        while (!stoppingToken.IsCancellationRequested)
        {
            {
                if (readerLogs.TryRead(out var itemLog))
                {
                    foreach (var itemScopeLog in itemLog.ScopeLogs)
                    {
                        foreach (var itemLogRecord in itemScopeLog.LogRecords)
                        {
                            this._RuleEngine.EnrichLog(new OneLogRecord(itemLog.Resource, itemLogRecord));
                        }
                    }
                    // TODO: here
                    await this._Sink.SendLog(itemLog);
                    await this._Sink.SendDisplayMessage(itemLog.ToString());
                    continue;
                }
            }
            
            await readerLogs.WaitToReadAsync(stoppingToken);
        }
    }

    //private string GetResourceServiceName(global::OpenTelemetry.Proto.Resource.V1.Resource resource)
    //{
    //    // { "key": "service.name", "value": { "stringValue": "ExampleWebApp" } },
    //    try
    //    {
    //        foreach (var attribute in resource.Attributes)
    //        {
    //            if (string.Equals("service.name", attribute.Key, StringComparison.OrdinalIgnoreCase))
    //            {
    //                return attribute.Value.StringValue;
    //            }
    //        }
    //    }
    //    catch
    //    { 
    //    }
    //    return string.Empty;
    //}

    //private string GetScopeName(global::OpenTelemetry.Proto.Common.V1.InstrumentationScope scope)
    //{
    //    global::Google.Protobuf.Collections.RepeatedField<global::OpenTelemetry.Proto.Common.V1.KeyValue> attributes;
    //    try
    //    {
    //        attributes = scope.Attributes;
    //    }
    //    catch (System.NullReferenceException)
    //    {
    //        return string.Empty;
    //    }

    //    foreach (var attribute in attributes)
    //    {
    //        if (string.Equals("name", attribute.Key, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return attribute.Value.StringValue;
    //        }
    //    }
    //    return string.Empty;
    //}

    //public Task AddLogs(DateTimeOffset utcNow, ExportLogsServiceRequest request, CancellationToken stopToken)
    //{
    //    var sb = new StringBuilder();
    //    foreach (var resourceLog in request.ResourceLogs)
    //    {
    //        var resourceServiceName = this.GetResourceServiceName(resourceLog.Resource);
    //        sb.AppendLine($"L1: {utcNow:O} {resourceServiceName} {resourceLog.Resource}");
    //        foreach (var scopeLog in resourceLog.ScopeLogs)
    //        {
    //            var scopeName = "";  //this.GetScopeName(scopeLog.Scope);
    //            sb.AppendLine($"L2: {utcNow:O} {resourceServiceName} {scopeName} {scopeLog.Scope}");
    //            foreach (var logRecord in scopeLog.LogRecords)
    //            {
    //                // logRecord.SeverityNumber
    //                // logRecord.Body
    //                // logRecord.SpanId
    //                // logRecord.TimeUnixNano
    //                //foreach (var attribute in logRecord.Attributes) {
    //                //    attribute.Key
    //                //    attribute.Value
    //                //}
    //                sb.AppendLine($"L3: {utcNow:O} {resourceServiceName} {scopeName} {logRecord}");
    //            }
    //        }
    //    }
    //    sb.AppendLine();
    //    System.Console.Out.WriteLine(sb.ToString());
    //    return Task.CompletedTask;
    //}

    //public Task AddMetrics(DateTimeOffset utcNow, ExportMetricsServiceRequest request, CancellationToken stopToken)
    //{
    //    var sb = new StringBuilder();
    //    foreach (var resourceMetric in request.ResourceMetrics)
    //    {
    //        var resourceServiceName = this.GetResourceServiceName(resourceMetric.Resource);
    //        sb.AppendLine($"M1: {utcNow:O} {resourceServiceName} {resourceMetric.Resource}");
    //        foreach (var scopeMetric in resourceMetric.ScopeMetrics)
    //        {
    //            var scopeName = this.GetScopeName(scopeMetric.Scope);
    //            sb.AppendLine($"M2: {utcNow:O} {resourceServiceName} {scopeName} {scopeMetric.Scope}");
    //            foreach (var metric in scopeMetric.Metrics)
    //            {
    //                sb.AppendLine($"M3: {utcNow:O} {resourceServiceName} {scopeName} {metric}");
    //            }
    //        }
    //    }
    //    sb.AppendLine();
    //    System.Console.Out.WriteLine(sb.ToString());
    //    return Task.CompletedTask;
    //}

    //public Task AddTrace(DateTimeOffset utcNow, ExportTraceServiceRequest request, CancellationToken stopToken)
    //{
    //    var sb = new StringBuilder();
    //    foreach (var resourceSpan in request.ResourceSpans)
    //    {
    //        var resourceServiceName = this.GetResourceServiceName(resourceSpan.Resource);
    //        sb.AppendLine($"T1: {utcNow:O} {resourceServiceName} {resourceSpan.Resource}");
    //        foreach (var scopeSpan in resourceSpan.ScopeSpans)
    //        {
    //            var scopeName = this.GetScopeName(scopeSpan.Scope);
    //            sb.AppendLine($"T2: {utcNow:O} {resourceServiceName} {scopeName} {scopeSpan.Scope}");
    //            foreach (var span in scopeSpan.Spans)
    //            {
    //                sb.AppendLine($"T3: {utcNow:O} {resourceServiceName} {scopeName} {span}");
    //            }
    //        }

    //    }
    //    sb.AppendLine();
    //    System.Console.Out.WriteLine(sb.ToString());
    //    return Task.CompletedTask;
    //}
}
