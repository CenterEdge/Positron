using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Positron.Server
{
    /// <summary>
    /// Positron-specific implementation of <see cref="UrlHelper"/> which treats ~/ urls as being specific to the
    /// current assembly rather than the entire application for Content URLs.
    /// </summary>
    public class PositronUrlHelper : UrlHelper
    {
        /// <summary>
        /// Creates a new <see cref="PositronUrlHelper"/> for a given <see cref="ActionContext"/>.
        /// </summary>
        /// <param name="actionContext"><see cref="ActionContext"/> used to extract route values.</param>
        public PositronUrlHelper(ActionContext actionContext) : base(actionContext)
        {
        }

        /// <inheritdoc />
        public override string Content(string contentPath)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                return null;
            }
            else if (contentPath[0] == '~')
            {
                var segment = new PathString(contentPath.Substring(1));
                var applicationPath = HttpContext.Request.PathBase;

                var assembly = ActionContext.RouteData.Values["assembly"] as string;
                if (!string.IsNullOrEmpty(assembly))
                {
                    applicationPath = applicationPath.Add(new PathString("/" + assembly));
                }

                return applicationPath.Add(segment).Value;
            }

            return contentPath;
        }
    }
}
