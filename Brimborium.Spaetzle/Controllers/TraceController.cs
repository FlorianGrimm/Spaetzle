namespace Brimborium.Spaetzle.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TraceController : ControllerBase
{
    public TraceController()
    {
    }

    [HttpGet]
    public List<global::OpenTelemetry.Proto.Trace.V1.ResourceSpans> GetResourceSpans()
    {
        return new();
    }
}
