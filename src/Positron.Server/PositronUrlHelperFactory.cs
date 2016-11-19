using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Positron.Server
{
    public class PositronUrlHelperFactory : IUrlHelperFactory
    {
        private const string PropertyOfTypeCannotBeNull = "Property {1} of type {0} cannot be null.";

        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext == null)
            {
                throw new ArgumentException(string.Format(PropertyOfTypeCannotBeNull,
                    nameof(ActionContext.HttpContext),
                    nameof(ActionContext)));
            }

            if (httpContext.Items == null)
            {
                throw new ArgumentException(string.Format(PropertyOfTypeCannotBeNull,
                    nameof(HttpContext.Items),
                    nameof(HttpContext)));
            }

            // Perf: Create only one UrlHelper per context
            object value;
            if (httpContext.Items.TryGetValue(typeof(IUrlHelper), out value) && value is IUrlHelper)
            {
                return (IUrlHelper)value;
            }

            var urlHelper = new PositronUrlHelper(context);
            httpContext.Items[typeof(IUrlHelper)] = urlHelper;

            return urlHelper;
        }
    }
}
