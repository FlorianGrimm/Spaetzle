using Brimborium.Spaetzle;
using Brimborium.Spaetzle.Contracts;

namespace Examples.Spaetzle;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddSpaetzle();
        builder.AddSpaetzleRules<ExamplesRules1>();
        builder.AddSpaetzleRules<ExamplesRules2>();
        var app = builder.Build();
        app.UseSpaetzle();
        app.Run();
    }
}

public class ExamplesRules1 : IRulesCollection
{
    public IEnumerable<IRule> GetRules() { 
        var result= new List<IRule>();
        return result;
    }
}

public class ExamplesRules2 : IRulesCollection
{
    public IEnumerable<IRule> GetRules()
    {
        var result = new List<IRule>();
        return result;
    }
}
