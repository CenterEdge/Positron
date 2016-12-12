using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positron.UI
{
    /// <summary>
    /// Base class for a resource request filter that filters based on scheme and host.
    /// </summary>
    public abstract class HostResourceRequestFilter : IResourceRequestFilter
    {
        /// <summary>
        /// List of valid schemes, i.e. "http" and "https".
        /// </summary>
        public abstract HashSet<string> ValidSchemes { get; }

        /// <summary>
        /// List of valid hosts, i.e. "positron" or "google.com".
        /// </summary>
        public abstract HashSet<string> ValidHosts { get; }

        /// <inheritdoc cref="IResourceRequestFilter"/>
        public virtual Task<bool> CanLoadResourceAsync(ResourceRequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Url == null)
            {
                throw new ArgumentException("ResourceRequestContext.Url may not be null.", nameof(context));
            }
            if (!context.Url.IsAbsoluteUri)
            {
                throw new ArgumentException("ResourceRequestContext.Url must be absolute.", nameof(context));
            }

            var result = ValidSchemes.Contains(context.Url.Scheme) && ValidHosts.Contains(context.Url.Host);

            if (!result)
            {
                OnResourceRejection(context);
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// Called when a resource is rejected to permit logging.
        /// </summary>
        /// <param name="context"><see cref="ResourceRequestContext"/> of the resource being rejected.</param>
        public virtual void OnResourceRejection(ResourceRequestContext context)
        {
        }
    }
}
