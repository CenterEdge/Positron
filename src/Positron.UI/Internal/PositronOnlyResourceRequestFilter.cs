using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using Microsoft.Extensions.Logging;

namespace Positron.UI.Internal
{
    internal class PositronOnlyResourceRequestFilter : HostResourceRequestFilter
    {
        private readonly ILogger<PositronOnlyResourceRequestFilter> _logger;

        public override HashSet<string> ValidSchemes { get; } = new HashSet<string> {"http"};
        public override HashSet<string> ValidHosts { get; } = new HashSet<string> {"positron"};

        public PositronOnlyResourceRequestFilter(ILogger<PositronOnlyResourceRequestFilter> logger)
        {
            _logger = logger;
        }

        public override void OnResourceRejection(ResourceRequestContext context)
        {
            _logger.LogWarning(LoggerEventIds.ExternalResource, "Preventing load of external resource '{0}'", context.Url);
        }
    }
}
