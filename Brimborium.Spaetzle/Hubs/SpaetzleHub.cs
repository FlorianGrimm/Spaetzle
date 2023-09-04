using Microsoft.AspNetCore.SignalR;

namespace Brimborium.Spaetzle.Hubs;

public interface ISpaetzleHubServer
{
    Task DisplayMessage(string message);
}

public interface ISpaetzleHub
{
    Task DisplayMessage(string message);
}
public record SubscripeStreamRequest(bool? logs, bool? traces, bool? metrics);

public class SpaetzleHub : Hub<ISpaetzleHub>, ISpaetzleHubServer
{
    public SpaetzleHub()
    {
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await this.UnsubscripeStream();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task DisplayMessage(string message)
    {
        await this.Clients.All.DisplayMessage(message);
    }


    public async Task SubscripeStream(SubscripeStreamRequest request)
    {
        if (request.logs.HasValue)
        {
            if (request.logs.Value)
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, "Logs");
                await this.DisplayMessage($"add to Logs {this.Context.ConnectionId}");
            } 
            else
            {
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "Logs");
                await this.DisplayMessage($"remove from Logs {this.Context.ConnectionId}");
            }
        }
        if (request.traces.HasValue)
        {
            if (request.traces.Value)
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, "Traces");
            } else
            {
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "Traces");
            }
        }
        if (request.metrics.HasValue)
        {
            if (request.metrics.Value)
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, "Metrics");
            } else
            {
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "Metrics");
            }
        }
    }

    public async Task UnsubscripeStream()
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "Logs");
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "Traces");
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, "Metrics");
    }

    //public async Task<string> Ask(string message) {
    //    return await this.Clients.Caller.Ask(message);
    //}
}

public class SpaetzleHubSink : ISpaetzleHubSink
{
    private readonly IHubContext<SpaetzleHub, ISpaetzleHub> _SpaetzleHub;

    public SpaetzleHubSink(
         IHubContext<SpaetzleHub, ISpaetzleHub> spaetzleHub
        )
    {
        this._SpaetzleHub = spaetzleHub;
    }

    public async Task SendDisplayMessage(string message)
    {
        await this._SpaetzleHub.Clients.All.DisplayMessage(message);
    }
}