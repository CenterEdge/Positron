using System;
using System.Windows;
using System.Windows.Interop;
using CefSharp;
using CefSharp.Wpf;

namespace Positron.UI
{
    public class LifeSpanHandler : ILifeSpanHandler
    {
        private readonly IWindowHandler _windowHandler;

        public LifeSpanHandler(IWindowHandler windowHandler)
        {
            if (windowHandler == null)
            {
                throw new ArgumentNullException(nameof(windowHandler));
            }

            _windowHandler = windowHandler;
        }

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName,
            WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo,
            IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;

            if ((targetDisposition == WindowOpenDisposition.NewForegroundTab) ||
                (targetDisposition == WindowOpenDisposition.NewBackgroundTab) ||
                (targetDisposition == WindowOpenDisposition.NewWindow))
            {
                var chromiumWebBrowser = (ChromiumWebBrowser) browserControl;
                ChromiumWebBrowser newWpfBrowser = null;

                chromiumWebBrowser.Dispatcher.Invoke(() =>
                {
                    var newWindow = _windowHandler.CreateWindow(Window.GetWindow(chromiumWebBrowser), targetUrl);
                    newWpfBrowser = (ChromiumWebBrowser) newWindow.Content;

                    newWindow.Owner.IsEnabled = false;

                    var windowInteropHelper = new WindowInteropHelper(newWindow);
                    //Create the handle Window handle (In WPF there's only one handle per window, not per control)
                    var handle = windowInteropHelper.EnsureHandle();

                    windowInfo.SetAsWindowless(handle, true);

                    newWindow.Closed += (o, e) =>
                    {
                        var window = o as Window;
                        var closingBrowser = window?.Content as IWebBrowser;
                        if (closingBrowser != null)
                        {
                            closingBrowser.Dispose();
                            window.Content = null;
                        }

                        var openingWindow = window?.Owner;
                        if (openingWindow != null)
                        {
                            openingWindow.IsEnabled = true;

                            var openingBrowser = openingWindow.Content as IWebBrowser;
                            if (openingBrowser != null)
                            {
                                openingBrowser.ExecuteScriptAsync(
                                    $@"if (dialogHandler && dialogHandler.complete) dialogHandler.complete();");
                            }
                        }
                    };
                });

                newBrowser = newWpfBrowser;
            }

            return false;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            var chromiumWebBrowser = (ChromiumWebBrowser) browserControl;

            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                var owner = Window.GetWindow(chromiumWebBrowser);

                if (owner != null && owner.Content == browserControl)
                {
                    owner.Show();
                }
            });
        }

        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            if (!chromiumWebBrowser.Dispatcher.HasShutdownStarted)
            {
                chromiumWebBrowser.Dispatcher.InvokeAsync(() =>
                {
                    var owner = Window.GetWindow(chromiumWebBrowser);

                    if (owner != null && owner.Content == browserControl)
                    {
                        owner.Close();
                    }
                });
            }
        }
    }
}
