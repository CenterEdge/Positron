using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Positron.UI
{
    internal class CefHeaderDictionary : HeaderDictionary
    {
        public CefHeaderDictionary(NameValueCollection headers)
            : base(headers.Count)
        {
            foreach (var key in headers.AllKeys)
            {
                Add(key, new StringValues(headers[key]));
            }
        }
    }
}
