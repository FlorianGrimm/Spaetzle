using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Diagnostics;

namespace Examples.AspNetCoreWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string? RequestId { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            this._logger = logger;
        }

        public void OnGet()
        {
            this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
            this._logger.LogTrace("LogTrace RequestId {RequestId}", this.RequestId);
            this._logger.LogDebug("LogDebug RequestId {RequestId}", this.RequestId);
            this._logger.LogInformation("LogInformation RequestId {RequestId}", this.RequestId);
            this._logger.LogWarning("LogWarning RequestId {RequestId}", this.RequestId);
            this._logger.LogError("LogError RequestId {RequestId}", this.RequestId);
            this._logger.LogCritical("LogCritical RequestId {RequestId}", this.RequestId);
        }
    }
}
