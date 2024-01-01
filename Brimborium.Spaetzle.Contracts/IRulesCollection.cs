namespace Brimborium.Spaetzle.Contracts;

public interface IRulesCollection {
    IEnumerable<IRule> GetRules();
}

public record struct OneLogRecord(
    Resource Resource,
    LogRecord LogRecord
    );

public record struct OneMetric(
    Resource Resource,
    InstrumentationScope Scope,
    Metric MetricRecord);

public record struct OneTrace(
    ResourceSpans Spans,
    InstrumentationScope Scope,
    Span TraceSpan);

public interface IRule {
    public string Name { get; }
    public int Order => 0;


}
public interface IRuleLog : IRule {
    void EnrichLog(OneLogRecord onLogRecord);

}
public interface IRuleMetric : IRule {
    void EnrichMetric(OneMetric oneMetric);
}
public interface IRuleTrace : IRule {
    void EnrichTrace(OneTrace oneTrace);
}