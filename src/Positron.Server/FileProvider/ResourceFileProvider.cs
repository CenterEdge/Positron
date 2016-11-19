using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Positron.Server.FileProvider
{
    public class ResourceFileProvider : IFileProvider
    {
        private readonly IAssemblyIdentifierProvider _assemblyIdentifierProvider;

        private readonly ConcurrentDictionary<string, AssemblyResourceSet> _resourceSets =
            new ConcurrentDictionary<string, AssemblyResourceSet>();

        public ResourceFileProvider(IAssemblyIdentifierProvider assemblyIdentifierProvider)
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

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}
