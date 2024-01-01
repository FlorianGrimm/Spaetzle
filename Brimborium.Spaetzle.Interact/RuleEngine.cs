namespace Brimborium.Spaetzle.Interact;

public interface IRuleEngine {
    // void AddRulesCollection(IRulesCollection rulesCollection);
    // void AddRule(IRule rule);
    void EnrichLog(OneLogRecord onLogRecord);
    void EnrichMetric(OneMetric oneMetric);
    void EnrichTraces(OneTrace oneTrace);
}

public class RuleEngine : IRuleEngine {
    public static RuleEngine Create(IServiceProvider services) {
        var result = new RuleEngine();
        var listRulesCollection = services.GetServices<IRulesCollection>();
        foreach (var rulesCollection in listRulesCollection) {
            result.AddRulesCollection(rulesCollection);
        }
        return result;
    }

    private List<IRuleLog> _ListRuleLog = new();
    private List<IRuleMetric> _ListRuleMetric = new();
    private List<IRuleTrace> _ListRuleTrace = new();

    public RuleEngine() {
    }

    public void AddRulesCollection(IRulesCollection rulesCollection) {
        var listRule = rulesCollection.GetRules();

        var listRuleLogTarget = new List<IRuleLog>(this._ListRuleLog);
        var listRuleMetricTarget = new List<IRuleMetric>(this._ListRuleMetric);
        var listRuleTraceTarget = new List<IRuleTrace>(this._ListRuleTrace);

        foreach (var rule in listRule) {
            if (rule is IRuleLog ruleLog) { listRuleLogTarget.Add(ruleLog); }
            if (rule is IRuleMetric ruleMetric) { listRuleMetricTarget.Add(ruleMetric); }
            if (rule is IRuleTrace ruleTrace) { listRuleTraceTarget.Add(ruleTrace); }
        }
        this.SortListRule(
            listRuleLogTarget,
            listRuleMetricTarget,
            listRuleTraceTarget
            );

    }

    public void AddRule(IRule rule) {
        var listRuleLogTarget = new List<IRuleLog>(this._ListRuleLog);
        var listRuleMetricTarget = new List<IRuleMetric>(this._ListRuleMetric);
        var listRuleTraceTarget = new List<IRuleTrace>(this._ListRuleTrace);
        //
        if (rule is IRuleLog ruleLog) { listRuleLogTarget.Add(ruleLog); }
        if (rule is IRuleMetric ruleMetric) { listRuleMetricTarget.Add(ruleMetric); }
        if (rule is IRuleTrace ruleTrace) { listRuleTraceTarget.Add(ruleTrace); }
        //
        this.SortListRule(
            listRuleLogTarget,
            listRuleMetricTarget,
            listRuleTraceTarget
            );
    }

    private ComparerIRule? _ComparerIRule;
    private void SortListRule(
        List<IRuleLog> listRuleLogTarget,
        List<IRuleMetric> listRuleMetricTarget,
        List<IRuleTrace> listRuleTraceTarget
        ) {
        var comparerIRule = this._ComparerIRule ??= new ComparerIRule();
        //
        listRuleLogTarget.Sort(comparerIRule);
        listRuleMetricTarget.Sort(comparerIRule);
        listRuleTraceTarget.Sort(comparerIRule);
        //
        this._ListRuleLog = listRuleLogTarget;
        this._ListRuleMetric = listRuleMetricTarget;
        this._ListRuleTrace = listRuleTraceTarget;
    }

    private sealed class ComparerIRule : IComparer<IRule> {
        public int Compare(IRule? x, IRule? y) {
            if (ReferenceEquals(x, y)) { return 0; }
            if (ReferenceEquals(x, null)) { return 0; }
            if (ReferenceEquals(y, null)) { return 0; }
            return x.Order - y.Order;
        }
    }


    public void EnrichLog(OneLogRecord onLogRecord) {
        var listRule = this._ListRuleLog;
        foreach (var rule in listRule) {
            rule.EnrichLog(onLogRecord);
        }
    }

    public void EnrichMetric(OneMetric oneMetric) {
        var listRule = this._ListRuleMetric;
        foreach (var rule in listRule) {
            rule.EnrichMetric(oneMetric);
        }
    }

    public void EnrichTraces(OneTrace oneTrace) {
        var listRule = this._ListRuleTrace;
        foreach (var rule in listRule) {
            rule.EnrichTrace(oneTrace);
        }
    }
}