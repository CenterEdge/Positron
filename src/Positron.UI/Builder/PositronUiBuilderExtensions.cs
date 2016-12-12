using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Positron.UI.Builder
{
    /// <summary>
    /// Extensions for <see cref="IPositronUiBuilder"/>.
    /// </summary>
    public static class PositronUiBuilderExtensions
    {
        /// <summary>
        /// Sets the <see cref="IConsoleLogger"/> used to handle Chromium console messages.
        /// </summary>
        /// <param name="builder">The <see cref="IPositronUiBuilder"/>.</param>
        /// <param name="consoleLogger"><see cref="IConsoleLogger"/> used to handle Chromium console messages.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        public static IPositronUiBuilder UseConsoleLogger(this IPositronUiBuilder builder, IConsoleLogger consoleLogger)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton(consoleLogger);
            });
        }

        /// <summary>
        /// Configures the debug port used to support Chromium developer tools.
        /// </summary>
        /// <param name="builder">The <see cref="IPositronUiBuilder"/>.</param>
        /// <param name="debugPort">Debug port used to support Chromium developer tools.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        /// <remarks>Access developer tools using http://localhost:xxxx from Chrome.</remarks>
        public static IPositronUiBuilder UseDebugPort(this IPositronUiBuilder builder, int debugPort)
        {
            return builder.ConfigureSettings(settings =>
            {
                settings.RemoteDebuggingPort = debugPort;
            });
        }
    }
}
