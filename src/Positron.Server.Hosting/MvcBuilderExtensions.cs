using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Extensions for <see cref="IMvcBuilder"/> for Positron.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds application parts for all assemblies in the entry assembly's path which reference
        /// MVC and pass a predicate.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="filePredicate">Predicate which receives the full path to the file and returns true to include the assembly.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddApplicationParts(this IMvcBuilder builder, Func<string, bool> filePredicate)
        {
            return AddApplicationParts(builder, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                filePredicate);
        }

        /// <summary>
        /// Adds application parts for all assemblies in the given path which reference
        /// MVC and pass a predicate.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="path">Path to scan for assemblies.</param>
        /// <param name="filePredicate">Predicate which receives the full path to the file and returns true to include the assembly.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddApplicationParts(this IMvcBuilder builder, string path,
            Func<string, bool> filePredicate)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (filePredicate == null)
            {
                throw new ArgumentNullException(nameof(filePredicate));
            }

            foreach (var file in Directory.GetFiles(path, "*.exe").Concat(Directory.GetFiles(path, "*.dll")))
            {
                if (filePredicate(file))
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(file);

                        if (assembly.GetReferencedAssemblies().Any(p => p.Name == "Microsoft.AspNetCore.Mvc.Core"))
                        {
                            builder = builder.AddApplicationPart(assembly);
                        }
                    }
                    catch
                    {
                        // Ignore assemblies that fail to load
                    }
                }
            }

            return builder;
        }
    }
}
