using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server.Hosting
{
    public interface IInternalHttpRequestFeature
    {
        Task<IHttpResponseFeature> ProcessRequestAsync(IHttpRequestFeature request);
    }
}
