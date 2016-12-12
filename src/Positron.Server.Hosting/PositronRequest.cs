using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Simple wrapper class for a request to submit via <see cref="IInternalHttpRequestFeature"/>.
    /// </summary>
    public class PositronRequest : IHttpRequestFeature
    {
        /// <inheritdoc />
        public string Protocol { get; set; }

        /// <inheritdoc />
        public string Scheme { get; set; }

        /// <inheritdoc />
        public string Method { get; set; }

        /// <inheritdoc />
        public string PathBase { get; set; }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public string QueryString { get; set; }

        /// <inheritdoc />
        public string RawTarget { get; set; }

        /// <inheritdoc />
        public IHeaderDictionary Headers { get; set; }

        /// <inheritdoc />
        public Stream Body { get; set; }
    }
}
