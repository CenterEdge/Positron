using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positron.UI
{
    /// <summary>
    /// Information about a Chromium request before it is processed.
    /// </summary>
    public class ResourceRequestContext
    {
        /// <summary>
        /// Request method, i.e. "GET" or "POST".
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// <see cref="Uri"/> of the referrer, if any.
        /// </summary>
        public Uri Referrer { get; set; }

        /// <summary>
        /// <see cref="Uri"/> being requested.  Should be an absolute Uri.
        /// </summary>
        public Uri Url { get; set; }
    }
}
