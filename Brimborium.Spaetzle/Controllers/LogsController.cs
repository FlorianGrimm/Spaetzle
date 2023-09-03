namespace Brimborium.Spaetzle.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    public LogsController()
    {
    }

    [HttpGet]
    public List<global::OpenTelemetry.Proto.Logs.V1.ResourceLogs> GetResourceLogs()
    {
        return new();
    }
}
