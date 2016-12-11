using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Positron.Server.Hosting;
using Positron.UI.Internal;

namespace Positron.UI
{
    class DisplayHandler : IDisplayHandler
    {
        private readonly PositronWindow _window;
        private readonly IConsoleLogger _consoleLogger;
        private readonly IWebHost _webHost;
        private readonly ILogger<DisplayHandler> _logger;

        private CancellationTokenSource _previousFaviconRequest;

        public DisplayHandler(PositronWindow window, IConsoleLogger consoleLogger, IWebHost webHost, ILogger<DisplayHandler> logger)
        {
            _window = window;
            _consoleLogger = consoleLogger;
            _webHost = webHost;
            _logger = logger;
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
            if (_previousFaviconRequest != null)
            {
                _previousFaviconRequest.Cancel();
            }

            _previousFaviconRequest = new CancellationTokenSource();
            var cancellationToken = _previousFaviconRequest.Token;

            _window.Dispatcher.InvokeAsync(async () =>
            {
                foreach (var uriString in urls)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Stop loop if cancelled
                        break;
                    }

                    try
                    {
                        BitmapImage image;

                        if (uriString.StartsWith("http://positron/"))
                        {
                            image = await LoadPositronImage(new Uri(uriString));
                        }
                        else
                        {
                            // For normal urls, use built in WPF image loader
                            image = new BitmapImage(new Uri(uriString));
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            // Don't set image if we've cancelled, let new task set new image
                            break;
                        }

                        _window.Icon = image;

                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(LoggerEventIds.FaviconError, ex, "Unable to load favicon {0}", uriString);

                        // Ignore error and try next url
                    }
                }
            }, DispatcherPriority.Normal, cancellationToken);
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

        private async Task<BitmapImage> LoadPositronImage(Uri uri)
        {
            using (var client = new HttpClient(new PositronInterceptingHttpHandler(_webHost)))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = await client.GetStreamAsync(uri);
                image.EndInit();

                return image;
            }
        }
    }
}
