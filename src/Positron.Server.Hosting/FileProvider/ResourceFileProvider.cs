using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Positron.Server.Hosting.FileProvider
{
    /// <summary>
    /// Positron implementation of <see cref="IFileProvider"/> that reads files from assembly resources.
    /// </summary>
    public class ResourceFileProvider : IFileProvider
    {
        private readonly IPositronRouteIdentifierProvider _assemblyIdentifierProvider;

        private readonly ConcurrentDictionary<string, AssemblyResourceSet> _resourceSets =
            new ConcurrentDictionary<string, AssemblyResourceSet>();

        /// <summary>
        /// Creates a new <see cref="ResourceFileProvider"/>.
        /// </summary>
        /// <param name="assemblyIdentifierProvider"><see cref="IPositronRouteIdentifierProvider"/> to resolve route identifiers into assemblies.</param>
        public ResourceFileProvider(IPositronRouteIdentifierProvider assemblyIdentifierProvider)
        {
            if (assemblyIdentifierProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyIdentifierProvider));
            }

            _assemblyIdentifierProvider = assemblyIdentifierProvider;
        }

        private AssemblyResourceSet GetResourceSet(string subpath, out string trailingPath)
        {
            string assemblyIdentifier;

            var slashIndex = subpath.IndexOf('/');
            if (slashIndex < 0)
            {
                assemblyIdentifier = subpath;
                trailingPath = "";
            }
            else
            {
                assemblyIdentifier = subpath.Substring(0, slashIndex);

                if (slashIndex + 1 >= subpath.Length)
                {
                    trailingPath = "";
                }
                else
                {
                    trailingPath = subpath.Substring(slashIndex + 1);
                }
            }

            return _resourceSets.GetOrAdd(assemblyIdentifier, key =>
            {
                var assemblyPart = _assemblyIdentifierProvider.GetApplicationPart(assemblyIdentifier);

                return assemblyPart == null ? null : new AssemblyResourceSet(assemblyPart.Assembly);
            });
        }

        /// <inheritdoc cref="IFileProvider"/>
        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                throw new ArgumentNullException(nameof(subpath));
            }

            if ((subpath.Length > 0) && (subpath[0] == '/'))
            {
                subpath = subpath.Substring(1);
            }

            string trailingPath;
            var resourceSet = GetResourceSet(subpath, out trailingPath);

            return resourceSet == null ?
                new ResourceFileInfo(null, null, Path.GetFileName(subpath), null, false) :
                resourceSet.GetFileInfo(trailingPath);
        }

        /// <inheritdoc cref="IFileProvider"/>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == null)
            {
                throw new ArgumentNullException(nameof(subpath));
            }

            if ((subpath.Length > 0) && (subpath[0] == '/'))
            {
                subpath = subpath.Substring(1);
            }

            string trailingPath;
            var resourceSet = GetResourceSet(subpath, out trailingPath);

            return resourceSet == null ?
                new ResourceDirectoryContents(null, null) :
                resourceSet.GetDirectoryContents(trailingPath);
        }

        /// <inheritdoc cref="IFileProvider"/>
        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}
