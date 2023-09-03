namespace Brimborium.Spaetzle.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MetricsController : ControllerBase
{
    public MetricsController()
    {
    }

    [HttpGet]
    public List<global::OpenTelemetry.Proto.Metrics.V1.ResourceMetrics> GetResourceMetrics()
    {
        return new();
    }
}
