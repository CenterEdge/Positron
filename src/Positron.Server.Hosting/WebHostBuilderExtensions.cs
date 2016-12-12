using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Positron.Server.Hosting.Internal;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Extensions to <see cref="IWebHostBuilder"/> for Positron.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Use the Positron server to process requests for the web host.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UsePositronServer(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.TryAddSingleton<IServer, PositronServer>();
            });

            return builder;
        }
    }
}
