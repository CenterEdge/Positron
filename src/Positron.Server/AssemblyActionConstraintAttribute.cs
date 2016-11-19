using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Positron.Server
{
    class AssemblyActionConstraintAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var reflectedAction = action as ControllerActionDescriptor;
            if (reflectedAction == null)
            {
                return true;
            }

            var identifierProvider = routeContext.HttpContext.RequestServices.GetService<IAssemblyIdentifierProvider>();
            var expectedAssembly = identifierProvider.GetIdentifier(reflectedAction.MethodInfo.DeclaringType?.Assembly?.GetName());

            return routeContext.RouteData.Values["assembly"]?.ToString().ToLowerInvariant() == expectedAssembly;
        }
    }
}
