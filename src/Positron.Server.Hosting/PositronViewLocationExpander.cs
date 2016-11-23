using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Positron.Server.Hosting
{
    public class PositronViewLocationExpander : IViewLocationExpander
    {
        private const string AssemblyIdentifierKey = "AssemblyIdentifier";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values[AssemblyIdentifierKey] = context.ActionContext.RouteData.Values["assembly"] as string;
        }

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
