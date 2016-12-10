using System;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Positron.Server;

namespace Positron.UI
{
    public class WindowHandler : IWindowHandler
    {
        private bool _globalScriptObjectsRegistered;

        public IServiceProvider Services { get; }

        public WindowHandler(IServiceProvider services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        private ChromiumWebBrowser CreateBrowser(string url)
        {
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
                foreach (var scriptObject in Services.GetServices<IGlobalScriptObject>())
                {
                    browser.RegisterJsObject(scriptObject.Name, scriptObject);
                }

                _globalScriptObjectsRegistered = true;
            }

            return browser;
        }

        public PositronWindow CreateWindow(Window owner, string url)
        {
            var browser = CreateBrowser(url);

            if (owner != null)
            {
                browser.SetAsPopup();
            }

            var newWindow = new PositronWindow
            {
                Owner = owner,
                Content = browser
            };

            browser.DisplayHandler = new DisplayHandler(newWindow,
                Services.GetService<IConsoleLogger>(),
                Services.GetRequiredService<IWebHost>());

            return newWindow;
        }

        public void Dispose()
        {
            Cef.Shutdown();
        }
    }
}
