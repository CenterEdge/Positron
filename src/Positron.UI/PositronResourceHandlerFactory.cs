using System;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Positron.UI
{
    class PositronResourceHandlerFactory : IResourceHandlerFactory
    {
        private readonly IWebHost _webHost;
        private readonly ILoggerFactory _loggerFactory;

        public bool HasHandlers => true;

        public PositronResourceHandlerFactory(IWebHost webHost, ILoggerFactory loggerFactory)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _webHost = webHost;
            _loggerFactory = loggerFactory;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            if (request.Url.StartsWith("http://positron/"))
            {
                return new PositronResourceHandler(_webHost, _loggerFactory.CreateLogger<PositronResourceHandler>());
            }

            return null;
        }
    }
}
