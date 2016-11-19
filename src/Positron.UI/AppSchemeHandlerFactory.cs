using CefSharp;
using Microsoft.AspNetCore.Hosting;

namespace Positron.UI
{
    class AppSchemeHandlerFactory : ISchemeHandlerFactory
    {
        private readonly IWebHost _webHost;

        public AppSchemeHandlerFactory(IWebHost webHost)
        {
            _webHost = webHost;
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new AppSchemeResourceHandler(_webHost);
        }
    }
}
