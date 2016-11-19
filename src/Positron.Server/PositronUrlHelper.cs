using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Positron.Server
{
    public class PositronUrlHelper : UrlHelper
    {
        public PositronUrlHelper(ActionContext actionContext) : base(actionContext)
        {
        }

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
