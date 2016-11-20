using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Positron.Server
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddControllersFromEntryPoint(this IMvcBuilder builder)
        {
            return AddControllersFromEntryPoint(builder, null);
        }

        public static IMvcBuilder AddControllersFromEntryPoint(this IMvcBuilder builder, Func<Assembly, bool> predicate)
        {
            var processed = new HashSet<Assembly>();

            AddControllersAndRecurse(builder, Assembly.GetEntryAssembly(), predicate, processed);

            return builder;
        }

        private static void AddControllersAndRecurse(IMvcBuilder builder, Assembly assembly,
            Func<Assembly, bool> predicate, HashSet<Assembly> processed)
        {
            if (processed.Contains(assembly))
            {
                // Already processed this assembly, skip
                return;
            }

            processed.Add(assembly);

            if ((predicate != null) && !predicate(assembly))
            {
                // This assembly and dependencies are excluded
                return;
            }

            if (assembly.GetReferencedAssemblies().Any(p => p.Name == "Microsoft.AspNetCore.Mvc.Core"))
            {
                builder.AddApplicationPart(assembly);
            }

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies()
                .Where(p => !p.Name.StartsWith("System") && !p.Name.StartsWith("Microsoft.") && (p.Name != "mscorlib")))
            {
                try
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);

                    AddControllersAndRecurse(builder, referencedAssembly, predicate, processed);
                }
                catch
                {
                    // ignore missing assemblies
                }
            }
        }
    }
}
