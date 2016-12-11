using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positron.UI.Builder
{
    public static class PositronUiBuilderExtensions
    {
        public static IPositronUiBuilder UseDebugPort(this IPositronUiBuilder builder, int debugPort)
        {
            return builder.ConfigureSettings(settings =>
            {
                settings.RemoteDebuggingPort = debugPort;
            });
        }
    }
}
