using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server
{
    public interface IInternalHttpRequestFeature
    {
        Task<IHttpResponseFeature> ProcessRequestAsync(IHttpRequestFeature request);
    }
}
