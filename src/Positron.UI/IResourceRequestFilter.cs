using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positron.UI
{
    /// <summary>
    /// Filter requests for resource load to provide application security
    /// </summary>
    public interface IResourceRequestFilter
    {
        /// <summary>
        /// Evaluates a request and returns true if the resource may be safely loaded in Chromium.
        /// </summary>
        /// <param name="context"><see cref="ResourceRequestContext"/> to evaluate.</param>
        /// <returns>True if the resource may be safely loaded in Chromium.</returns>
        Task<bool> CanLoadResourceAsync(ResourceRequestContext context);
    }
}
