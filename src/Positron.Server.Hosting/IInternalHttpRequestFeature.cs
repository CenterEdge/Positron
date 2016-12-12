using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server.Hosting
{
    /// <summary>
    /// Server feature which allows requests to be forwarded to the server in-process.
    /// </summary>
    public interface IInternalHttpRequestFeature
    {
        /// <summary>
        /// Processes a request on the server asynchronously.
        /// </summary>
        /// <param name="request"><see cref="IHttpRequestFeature"/> to process.</param>
        /// <returns><see cref="IHttpResponseFeature"/> containing the response.</returns>
        Task<IHttpResponseFeature> ProcessRequestAsync(IHttpRequestFeature request);
    }
}
