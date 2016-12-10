using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Positron.Server.Hosting
{
    internal class HttpClientHeaderDictionary : HeaderDictionary
    {
        public HttpClientHeaderDictionary(HttpHeaders headers)
        {
            foreach (var header in headers)
            {
                Add(header.Key, new StringValues(header.Value.ToArray()));
            }
        }
    }
}
