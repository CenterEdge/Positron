using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Positron.Server.Hosting
{
    public class PositronInterceptingHttpHandler : DelegatingHandler
    {
        private readonly IInternalHttpRequestFeature _requestFeature;

        public PositronInterceptingHttpHandler(IWebHost webHost) : this(webHost, new HttpClientHandler())
        {
        }

        public PositronInterceptingHttpHandler(IWebHost webHost, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            _requestFeature = webHost.ServerFeatures.Get<IInternalHttpRequestFeature>();
            if (_requestFeature == null)
            {
                throw new ArgumentException($"Provided ${nameof(IWebHost)} doesn't support internal requests.", nameof(webHost));
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.Scheme == "http" && request.RequestUri.Host == "positron")
            {
                return ProcessPositronRequest(request, cancellationToken);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> ProcessPositronRequest(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var internalRequest = new PositronRequest
            {
                Protocol = "HTTP/1.1",
                Method = request.Method.Method,
                Path = Uri.UnescapeDataString(request.RequestUri.AbsolutePath),
                QueryString = request.RequestUri.Query,
                Scheme = request.RequestUri.Scheme,
                Headers = new HttpClientHeaderDictionary(request.Headers)
            };

            var internalResponse = await _requestFeature.ProcessRequestAsync(internalRequest);
            cancellationToken.ThrowIfCancellationRequested();

            var response = new HttpResponseMessage((HttpStatusCode) internalResponse.StatusCode)
            {
                RequestMessage = request,
                ReasonPhrase = internalResponse.ReasonPhrase,
                Version = new Version(1, 1),
                Content = new StreamContent(internalResponse.Body)
            };

            var responseHeaders = response.Headers;
            var contentHeaders = response.Content.Headers;

            foreach (var header in internalResponse.Headers)
            {
                if (!responseHeaders.TryAddWithoutValidation(header.Key, (IEnumerable<string>) header.Value))
                    contentHeaders.TryAddWithoutValidation(header.Key, (IEnumerable<string>) header.Value);
            }

            return response;
        }
    }
}
