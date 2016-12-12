using System;

namespace Positron.Server
{
    /// <summary>
    /// Resolves Positron URIs into pack:// resource URIs.
    /// </summary>
    public interface IPositronResourceResolver
    {
        /// <summary>
        /// Resolves Positron URIs into pack:// resource URIs.
        /// </summary>
        /// <param name="input">Positron URI to resolve.</param>
        /// <returns>pack:// resource URI.</returns>
        Uri GetResourceUri(string input);
    }
}