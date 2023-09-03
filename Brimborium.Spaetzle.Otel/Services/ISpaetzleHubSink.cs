namespace Brimborium.Spaetzle.Otel.Services;

public interface ISpaetzleHubSink
{
    Task SendDisplayMessage(string message);
}
