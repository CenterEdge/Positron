using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Positron.Server.Hosting.FileProvider;

namespace Positron.Server.Hosting
{
    internal class ConfigureRazorViewEngineOptions : IConfigureOptions<RazorViewEngineOptions>
    {
        private readonly IServiceProvider _serviceProvider;

        public ConfigureRazorViewEngineOptions(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        public void Configure(RazorViewEngineOptions options)
        {
            options.FileProviders.Add(_serviceProvider.GetService<ResourceFileProvider>());
            options.ViewLocationExpanders.Add(new PositronViewLocationExpander());

            var references = new HashSet<Assembly>();
            CollectReferences(references, new[]
            {
                Assembly.GetEntryAssembly().GetName()
            });

            foreach (var reference in references)
            {
                options.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(reference.Location));
            }

            options.CompilationOptions = options.CompilationOptions.WithUsings(
                "Microsoft.AspNetCore.Html");
        }

        private void CollectReferences(HashSet<Assembly> hashSet, IEnumerable<AssemblyName> references)
        {
            foreach (var assemblyName in references)
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);

                    if (!hashSet.Contains(assembly))
                    {
                        hashSet.Add(assembly);

                        CollectReferences(hashSet, assembly.GetReferencedAssemblies());
                    }
                }
                catch
                {
                    // Skip bad assemblies
                }
            }
        }
    }
}
