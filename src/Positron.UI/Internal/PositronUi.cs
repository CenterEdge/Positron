using System;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Positron.UI.Internal
{
    internal class PositronUi : IPositronUi
    {
        private readonly ILogger<PositronUi> _logger;
        private bool _globalScriptObjectsRegistered;

        public IServiceProvider Services { get; }

        public PositronUi(IServiceProvider services, ILogger<PositronUi> logger)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Services = services;
            _logger = logger;

            _logger.LogInformation(LoggerEventIds.Startup, "CEF startup complete");
        }

        private ChromiumWebBrowser CreateBrowser(string url)
        {
            _logger.LogInformation(LoggerEventIds.CreateBrowser, "Creating browser for url '{0}'", url);

            var browser = new ChromiumWebBrowser
            {
                Address = url,
                RequestHandler = Services.GetRequiredService<IRequestHandler>(),
                LifeSpanHandler = Services.GetRequiredService<ILifeSpanHandler>(),
                KeyboardHandler = Services.GetRequiredService<IKeyboardHandler>(),
                ResourceHandlerFactory = Services.GetRequiredService<IResourceHandlerFactory>()
            };

            if (!_globalScriptObjectsRegistered)
            {
                _logger.LogInformation(LoggerEventIds.RegisterGlobalScriptObjects, "Registering IGlobalScriptObjects");

                var count = 0;
                foreach (var scriptObject in Services.GetServices<IGlobalScriptObject>())
                {
                    browser.RegisterJsObject(scriptObject.Name, scriptObject);

                    count += 1;
                }

                _logger.LogInformation(LoggerEventIds.RegisterGlobalScriptObjects, "Registered IGlobalScriptObjects, found {0} objects", count);

                _globalScriptObjectsRegistered = true;
            }

            return browser;
        }

        public PositronWindow CreateWindow(Window owner, string url)
        {
            var browser = CreateBrowser(url);

            if (owner != null)
            {
                _logger.LogDebug(LoggerEventIds.CreateWindow, "Window has owner, marking as popup");

                browser.SetAsPopup();
            }

            var newWindow = new PositronWindow
            {
                Owner = owner,
                Content = browser
            };

            browser.DisplayHandler = new DisplayHandler(newWindow,
                Services.GetService<IConsoleLogger>(),
                Services.GetRequiredService<IWebHost>(),
                Services.GetRequiredService<ILogger<DisplayHandler>>());

            _logger.LogDebug(LoggerEventIds.CreateWindow, "Window created for url '{0}'", url);

            return newWindow;
        }

        public void Dispose()
        {
            try
            {
                _logger.LogInformation(LoggerEventIds.Shutdown, "Shutting down CEF");

                Cef.Shutdown();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerEventIds.Shutdown, ex, "Error in CEF shutdown");
            }
        }

    }
}
