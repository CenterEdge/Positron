using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Positron.Server.Hosting.FileProvider;

namespace Positron.Server.Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePositron(this IApplicationBuilder builder)
        {
            return builder.UsePositron(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{assembly}/{controller=Home}/{action=Index}/{id?}");
            });
        }

        public static IApplicationBuilder UsePositron(this IApplicationBuilder builder, Action<IRouteBuilder> configureRoutes)
        {
            return builder
                .UseMvc(configureRoutes)
                .UseMiddleware<ResourceFileMiddleware>();
        }
    }
}
