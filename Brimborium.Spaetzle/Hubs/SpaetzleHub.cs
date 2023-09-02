using Microsoft.AspNetCore.SignalR;

namespace Brimborium.Spaetzle.Hubs;

public interface ISpaetzleHub
{
    Task DisplayMessage(string message);

    //Task<string> Ask(string message);
}
public class SpaetzleHub : Hub<ISpaetzleHub>
{
    public SpaetzleHub()
    {
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task DisplayMessage(string message)
    {
        await this.Clients.All.DisplayMessage(message);
    }

    //public async Task<string> Ask(string message) {
    //    return await this.Clients.Caller.Ask(message);
    //}
}

public class XHub : IXHub
{
    private readonly IHubContext<SpaetzleHub, ISpaetzleHub> _SpaetzleHub;

    public XHub(
         IHubContext<SpaetzleHub, ISpaetzleHub> spaetzleHub
        )
    {
        this._SpaetzleHub = spaetzleHub;
    }

    public async void AddLog(string body)
    {
        await this._SpaetzleHub.Clients.All.DisplayMessage(body);
    }
}