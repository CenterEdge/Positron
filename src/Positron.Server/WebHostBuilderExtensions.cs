using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Positron.Server
{
    public static class WebHostBuilderExtensions
    {
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
