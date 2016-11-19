using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Positron.Server.FileProvider;

namespace Positron.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IMvcBuilder AddUsePositronServer(this IServiceCollection collection)
        {
            return collection.AddUsePositronServer(null);
        }

        public static IMvcBuilder AddUsePositronServer(this IServiceCollection collection, Action<MvcOptions> setupAction)
        {
            collection.TryAddSingleton<IAssemblyIdentifierProvider, PositronAssemblyIdentifierProvider>();
            collection.TryAddSingleton<IAppSchemeResourceResolver, AppSchemeResourceResolver>();
            collection.TryAddTransient<IConfigureOptions<RazorViewEngineOptions>, ConfigureRazorViewEngineOptions>();
            collection.TryAddSingleton<IUrlHelperFactory, PositronUrlHelperFactory>();
            collection.TryAddSingleton<ResourceFileProvider>();

            return collection.AddMvc(options =>
            {
                options.Conventions.Add(new PositronModelConvention());

                setupAction?.Invoke(options);
            });
        }
    }
}
