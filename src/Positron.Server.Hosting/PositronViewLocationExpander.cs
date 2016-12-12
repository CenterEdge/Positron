using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Positron implementation of <see cref="IViewLocationExpander"/> which prefixes the possible
    /// view locations with the assembly identifier. This allows the view resources to be found in the
    /// assembly using <see cref="Positron.Server.Hosting.FileProvider.ResourceFileProvider"/>.
    /// </summary>
    public class PositronViewLocationExpander : IViewLocationExpander
    {
        private const string AssemblyIdentifierKey = "AssemblyIdentifier";

        /// <inheritdoc cref="IViewLocationExpander"/>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values[AssemblyIdentifierKey] = context.ActionContext.RouteData.Values["assembly"] as string;
        }

        /// <inheritdoc cref="IViewLocationExpander"/>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var assemblyIdentifier = context.Values[AssemblyIdentifierKey];
            if (string.IsNullOrEmpty(assemblyIdentifier))
            {
                return viewLocations;
            }
            else
            {
                return viewLocations.Select(p => "/" + assemblyIdentifier + p);
            }
        }
    }
}
