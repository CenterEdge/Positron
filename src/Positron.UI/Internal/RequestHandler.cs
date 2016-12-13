using System;
using System.Windows.Shell;
using CefSharp;
using Microsoft.Extensions.Logging;

namespace Positron.UI.Internal
{
    internal class RequestHandler : IRequestHandler
    {
        private readonly ILogger<RequestHandler> _logger;
        private readonly IResourceRequestFilter _requestFilter;

        public RequestHandler(ILogger<RequestHandler> logger, IResourceRequestFilter requestFilter)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (requestFilter == null)
            {
                throw new ArgumentNullException(nameof(requestFilter));
            }

            _logger = logger;
            _requestFilter = requestFilter;
        }

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            return false;
        }

        public virtual bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public virtual bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl,
            ISslInfo sslInfo, IRequestCallback callback)
        {
            callback.Dispose();
            return false;
        }

        public virtual void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
        }

        public virtual CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IRequestCallback callback)
        {
            var context = new ResourceRequestContext
            {
                Method = request.Method,
                Referrer = !string.IsNullOrEmpty(request.ReferrerUrl) ? new Uri(request.ReferrerUrl) : null,
                Url = new Uri(request.Url)
            };

            _requestFilter.CanLoadResourceAsync(context)
                .ContinueWith(task =>
                {
                    using (callback)
                    {
                        if (!task.IsCompleted)
                        {
                            if (task.Exception != null)
                            {
                                foreach (var ex in task.Exception.Flatten().InnerExceptions)
                                {
                                    _logger.LogError(LoggerEventIds.ResourceRequestFilterError, ex,
                                        "Error processing IResourceRequestFilter for url '{0}'", context.Url);
                                }
                            }
                            else
                            {
                                _logger.LogError(LoggerEventIds.ResourceRequestFilterError,
                                    "Error processing IResourceRequestFilter for url '{0}', no exception returned",
                                    context.Url);
                            }

                            callback.Cancel();
                        }
                        else
                        {
                            callback.Continue(task.Result);
                        }
                    }
                });

            return CefReturnValue.ContinueAsync;
        }

        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port,
            string realm, string scheme, IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize,
            IRequestCallback callback)
        {
            callback.Dispose();
            return false;
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
        {
        }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return false;
        }

        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response)
        {
            return null;
        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
        }
    }
}
