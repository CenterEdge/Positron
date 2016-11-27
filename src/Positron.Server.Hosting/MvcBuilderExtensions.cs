using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Positron.Server.Hosting
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddApplicationParts(this IMvcBuilder builder, Func<string, bool> filePredicate)
        {
            return AddApplicationParts(builder, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                filePredicate);
        }

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
