using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Positron.Server.Hosting.FileProvider;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Extensions to <see cref="IApplicationBuilder"/> for Positron.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use Positron using the default route ({assembly}/{controller=Home}/{action=Index}/{id?}).
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UsePositron(this IApplicationBuilder builder)
        {
            return builder.UsePositron(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{assembly}/{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// Use Positron using custom routes.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="configureRoutes">Action which will configure the MVC routes.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UsePositron(this IApplicationBuilder builder, Action<IRouteBuilder> configureRoutes)
        {
            return builder
                .UseMvc(configureRoutes)
                .UseMiddleware<ResourceFileMiddleware>();
        }
    }
}
