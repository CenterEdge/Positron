using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Positron.Server.Hosting.FileProvider;
using Positron.Server.Hosting.Internal;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> for Positron.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Positron and MVC related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection"><see cref="IServiceCollection"/> being built.</param>
        /// <returns><see cref="IMvcBuilder"/> for further customization.</returns>
        public static IMvcBuilder AddPositronServer(this IServiceCollection collection)
        {
            return collection.AddPositronServer(null);
        }

        /// <summary>
        /// Add Positron and MVC related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection"><see cref="IServiceCollection"/> being built.</param>
        /// <param name="setupAction">Action to perform additonal MVC configuration.</param>
        /// <returns><see cref="IMvcBuilder"/> for further customization.</returns>
        public static IMvcBuilder AddPositronServer(this IServiceCollection collection, Action<MvcOptions> setupAction)
        {
            collection.TryAddSingleton<IPositronRouteIdentifierProvider, PositronRouteIdentifierProvider>();
            collection.TryAddSingleton<IPositronResourceResolver, PositronResourceResolver>();
            collection.TryAddSingleton<IUrlHelperFactory, PositronUrlHelperFactory>();
            collection.TryAddSingleton<ResourceFileProvider>();

            var builder = collection.AddMvc(options =>
            {
                options.Conventions.Add(new PositronModelConvention());

                setupAction?.Invoke(options);
            });

            collection.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<RazorViewEngineOptions>, ConfigureRazorViewEngineOptions>());

            return builder;
        }
    }
}
