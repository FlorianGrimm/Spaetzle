using Cocona;

using Microsoft.Extensions.Logging;

namespace Examples.ConsoleIncludeSpaetzle;
//
// testing Brimborium.Spaetzle.OtelLocal
// 
public partial class Program {
    public static void Main(string[] args) {
        var builder = CoconaApp.CreateBuilder();
        var app = builder.Build();

        app.AddCommand((int a, int b, ILogger<Program> logger) => {
            logger.InformationÎnvokeCommand(a, b);
            logger.InformationFini();
        });

        app.Run();
    }
}

public static partial class ProgramLogging {
    [LoggerMessage(
        EventId = 1,
        EventName = "ÎnvokeCommand",
        Level = LogLevel.Information,
        Message = "Command {a}, {b}")]
    public static partial void InformationÎnvokeCommand(this ILogger logger, int a, int b);

    [LoggerMessage(
        EventId = 99,
        EventName = "Fini",
        Level = LogLevel.Information,
        Message = "- fini -")]
    public static partial void InformationFini(this ILogger logger);
}