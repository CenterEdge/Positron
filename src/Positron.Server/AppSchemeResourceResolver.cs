using System;

namespace Positron.Server
{
    public class AppSchemeResourceResolver : IAppSchemeResourceResolver
    {
        private readonly IAssemblyIdentifierProvider _assemblyIdentifierProvider;

        public AppSchemeResourceResolver(IAssemblyIdentifierProvider assemblyIdentifierProvider)
        {
            if (assemblyIdentifierProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyIdentifierProvider));
            }

            _assemblyIdentifierProvider = assemblyIdentifierProvider;
        }

        public Uri GetResourceUri(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.StartsWith("app://positron"))
            {
                input = input.Substring(11);
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
