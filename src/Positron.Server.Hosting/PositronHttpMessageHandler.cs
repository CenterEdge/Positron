using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Positron.Server.Hosting.Internal;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// <see cref="HttpMessageHandler"/> to use with <see cref="HttpClient"/> which routes Positron URLs
    /// to the Positron server.  All other URLs are handled via the provided HttpMessageHandler.
    /// </summary>
    public class PositronHttpMessageHandler : DelegatingHandler
    {
        private readonly IInternalHttpRequestFeature _requestFeature;

        /// <summary>
        /// Creates a <see cref="PositronHttpMessageHandler"/>, using <see cref="HttpClientHandler"/> as the fallback
        /// for generic URLs.
        /// </summary>
        /// <param name="webHost"><see cref="IWebHost"/> of a Positron server to receive requests.</param>
        /// <remarks>The <see cref="IWebHost"/> must support the <see cref="IInternalHttpRequestFeature"/> feature.</remarks>
        public PositronHttpMessageHandler(IWebHost webHost) : this(webHost, new HttpClientHandler())
        {
        }

        /// <summary>
        /// Creates a <see cref="PositronHttpMessageHandler"/>, using a provided handler as the fallback
        /// for generic URLs.
        /// </summary>
        /// <param name="webHost"><see cref="IWebHost"/> of a Positron server to receive requests.</param>
        /// <param name="innerHandler">Handler to use as a fallback for non-Positron requests.</param>
        /// <remarks>The <see cref="IWebHost"/> must support the <see cref="IInternalHttpRequestFeature"/> feature.</remarks>
        public PositronHttpMessageHandler(IWebHost webHost, HttpMessageHandler innerHandler)
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

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.Scheme == "http" && request.RequestUri.Host == "positron")
            {
                return ProcessPositronRequest(request, cancellationToken);
            }

            return base.SendAsync(request, cancellationToken);
        }

        /// <inheritdoc />
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
