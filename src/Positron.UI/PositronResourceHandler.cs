using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Positron.Server.Hosting;
using Cookie = CefSharp.Cookie;

namespace Positron.UI
{
    class PositronResourceHandler : IResourceHandler
    {
        private readonly IWebHost _webHost;
        private Uri _requestUri;
        private IHttpResponseFeature _response;

        public PositronResourceHandler(IWebHost webHost)
        {
            _webHost = webHost;
        }

        public void Dispose()
        {
        }

        public bool ProcessRequest(IRequest request, ICallback callback)
        {
            var url = new Uri(request.Url);
            _requestUri = url;

            var internalRequest = new PositronRequest
            {
                Protocol = "HTTP/1.1",
                Method = request.Method,
                Path = url.AbsolutePath,
                QueryString = url.Query,
                Scheme = "http",
                Headers = new CefHeaderDictionary(request.Headers)
            };

            if (request.PostData != null && request.PostData.Elements.Any())
            {
                internalRequest.Body = new MemoryStream(request.PostData.Elements.First().Bytes);
            }

            Task.Run(() =>
            {
                var processor = _webHost.ServerFeatures.Get<IInternalHttpRequestFeature>();

                // Do this in a task to ensure it doesn't execute synchronously on the ProcessRequest thread
                processor.ProcessRequestAsync(internalRequest)
                    .ContinueWith(task =>
                    {
                        using (callback)
                        {
                            _response = task.Result;

                            callback.Continue();
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion)
                    .ContinueWith(task =>
                    {
                        using (callback)
                        {
                            _response = null;

                            callback.Cancel();
                        }
                    }, TaskContinuationOptions.NotOnRanToCompletion)
                    .ContinueWith(task =>
                    {
                        internalRequest.Body?.Dispose();
                    });
            });

            return true;
        }

        public void GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            redirectUrl = null;
            responseLength = (_response.Body?.Length).GetValueOrDefault(-1);

            var mimeType = _response.Headers["Content-Type"].FirstOrDefault();
            if (mimeType != null)
            {
                var i = mimeType.IndexOf(';');
                if (i >= 0)
                {
                    mimeType = mimeType.Substring(0, i);
                }
            }

            response.StatusCode = _response.StatusCode;
            response.StatusText = _response.ReasonPhrase;
            response.MimeType = mimeType;

            response.ResponseHeaders = new NameValueCollection();
            foreach (var header in _response.Headers)
            {
                response.ResponseHeaders[header.Key] = header.Value.FirstOrDefault();
            }

            if ((response.StatusCode == (int) HttpStatusCode.Redirect) ||
                (response.StatusCode == (int) HttpStatusCode.TemporaryRedirect))
            {
                var redirectLocation = _response.Headers["Location"].FirstOrDefault();
                if (redirectLocation != null)
                {
                    try
                    {
                        var redirectLocationUri = new Uri(redirectLocation, UriKind.RelativeOrAbsolute);

                        if (!redirectLocationUri.IsAbsoluteUri)
                        {
                            redirectLocationUri = new Uri(_requestUri, redirectLocationUri);
                        }

                        redirectUrl = redirectLocationUri.ToString();
                    }
                    catch
                    {
                        // Bad url, ignore
                    }
                }
            }
        }

        public bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            callback.Dispose();

            if (_response.Body == null)
            {
                bytesRead = 0;
                return false;
            }

            var buffer = new byte[dataOut.Length];
            bytesRead = _response.Body.Read(buffer, 0, buffer.Length);

            dataOut.Write(buffer, 0, bytesRead);

            return bytesRead > 0;
        }

        public bool CanGetCookie(Cookie cookie)
        {
            return true;
        }

        public bool CanSetCookie(Cookie cookie)
        {
            return true;
        }

        public void Cancel()
        {
        }
    }
}
