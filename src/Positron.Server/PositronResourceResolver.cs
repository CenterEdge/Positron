using System;

namespace Positron.Server
{
    /// <summary>
    /// Resolves Positron URIs into pack:// resource URIs.
    /// </summary>
    public class PositronResourceResolver : IPositronResourceResolver
    {
        private readonly IPositronRouteIdentifierProvider _assemblyIdentifierProvider;

        /// <summary>
        /// Creates a new <see cref="PositronResourceResolver"/>.
        /// </summary>
        /// <param name="assemblyIdentifierProvider"><see cref="IPositronRouteIdentifierProvider"/> to identify assemblies for routes.</param>
        public PositronResourceResolver(IPositronRouteIdentifierProvider assemblyIdentifierProvider)
        {
            if (assemblyIdentifierProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyIdentifierProvider));
            }

            _assemblyIdentifierProvider = assemblyIdentifierProvider;
        }

        /// <summary>
        /// Resolves Positron URIs into pack:// resource URIs.
        /// </summary>
        /// <param name="input">Positron URI to resolve.</param>
        /// <returns>pack:// resource URI.</returns>
        public Uri GetResourceUri(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.StartsWith("http://positron"))
            {
                input = input.Substring(15);
            }

            if (input[0] != '/')
            {
                return null;
            }

            if (input.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (input.EndsWith(".vbhtml", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var index = input.IndexOf('/', 1);
            if (index < 0)
            {
                return null;
            }

            var assembly = input.Substring(1, index - 1);
            var path = input.Substring(index);

            assembly = _assemblyIdentifierProvider.GetApplicationPart(assembly)?.Assembly.GetName().Name;
            if (assembly == null)
            {
                return null;
            }

            return new Uri("pack://application:,,,/" + assembly + ";component" + path);
        }
    }
}
