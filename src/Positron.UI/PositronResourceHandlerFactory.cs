using CefSharp;
using Microsoft.AspNetCore.Hosting;

namespace Positron.UI
{
    class PositronResourceHandlerFactory : IResourceHandlerFactory
    {
        private readonly IWebHost _webHost;


        public bool HasHandlers => true;

        public PositronResourceHandlerFactory(IWebHost webHost)
        {
            _webHost = webHost;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            if (request.Url.StartsWith("http://positron/"))
            {
                return new PositronResourceHandler(_webHost);
            }

            return null;
        }
    }
}
