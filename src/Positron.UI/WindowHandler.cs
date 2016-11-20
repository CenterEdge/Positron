using System;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Positron.Server;

namespace Positron.UI
{
    public class WindowHandler : IWindowHandler
    {
        private readonly IAppSchemeResourceResolver _appSchemeResourceResolver;
        private readonly IConsoleLogger _consoleLogger;
        private bool _globalScriptObjectsRegistered;

        public IServiceProvider Services { get; }

        public WindowHandler(IServiceProvider services, IAppSchemeResourceResolver appSchemeResourceResolver,
            IConsoleLogger consoleLogger)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (appSchemeResourceResolver == null)
            {
                throw new ArgumentNullException(nameof(appSchemeResourceResolver));
            }

            Services = services;
            _appSchemeResourceResolver = appSchemeResourceResolver;
            _consoleLogger = consoleLogger;
        }

        private ChromiumWebBrowser CreateBrowser(string url)
        {
            var browser = new ChromiumWebBrowser
            {
                Address = url,
                RequestHandler = Services.GetRequiredService<IRequestHandler>(),
                LifeSpanHandler = Services.GetRequiredService<ILifeSpanHandler>()
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

            browser.DisplayHandler = new DisplayHandler(newWindow, _appSchemeResourceResolver, _consoleLogger);

            return newWindow;
        }
    }
}
