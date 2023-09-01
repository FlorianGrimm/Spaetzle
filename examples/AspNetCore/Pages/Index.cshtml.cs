using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Diagnostics;

namespace Examples.AspNetCore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _Logger;
        public DateTimeOffset Now { get; set; }
        public string ActivityId { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger)
        {
            this._Logger = logger;
            this.ActivityId = string.Empty;
        }

        public void OnGet()
        {
            this.Now = DateTimeOffset.Now;
            this.ActivityId = Activity.Current?.Id ?? "%";
            this._Logger.LogInformation("Hello {Now} {ActivityId}", this.Now, this.ActivityId);
        }
    }
}
