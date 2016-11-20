using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using CefSharp;
using Positron.Server;

namespace Positron.UI
{
    class DisplayHandler : IDisplayHandler
    {
        private readonly PositronWindow _window;
        private readonly IAppSchemeResourceResolver _appSchemeResourceResolver;
        private readonly IConsoleLogger _consoleLogger;

        public DisplayHandler(PositronWindow window, IAppSchemeResourceResolver appSchemeResourceResolver,
            IConsoleLogger consoleLogger)
        {
            _window = window;
            _appSchemeResourceResolver = appSchemeResourceResolver;
            _consoleLogger = consoleLogger;
        }

        public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
        {
        }

        public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
        {
            _window.Dispatcher.InvokeAsync(() => {
                _window.Title = titleChangedArgs.Title;
            });
        }

        public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
        {
            var uriString = urls.FirstOrDefault();
            if (uriString == null)
            {
                return;
            }

            if (uriString.StartsWith("app://positron"))
            {
                var uri = _appSchemeResourceResolver.GetResourceUri(uriString);

                _window.Dispatcher.InvokeAsync(() => {
                    _window.Icon = new BitmapImage(uri);
                });
            }
        }

        public void OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
        {
        }

        public bool OnTooltipChanged(IWebBrowser browserControl, string text)
        {
            return false;
        }

        public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
        {
        }

        public bool OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs)
        {
            try
            {
                _consoleLogger?.WriteMessage(consoleMessageArgs);
            }
            catch
            {
                // ignored
            }

            return true;
        }
    }
}
