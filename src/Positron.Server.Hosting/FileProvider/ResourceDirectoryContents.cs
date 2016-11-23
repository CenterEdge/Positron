using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace Positron.Server.Hosting.FileProvider
{
    internal class ResourceDirectoryContents : IDirectoryContents
    {
        private readonly AssemblyResourceSet _resourceSet;
        private readonly IList<string> _filePaths;

        public bool Exists => _filePaths != null;

        public ResourceDirectoryContents(AssemblyResourceSet resourceSet, IList<string> filePaths)
        {
            _resourceSet = resourceSet;
            _filePaths = filePaths;
        }

        private IEnumerable<IFileInfo> GetFileInfos()
        {
            return _filePaths.Select(p => _resourceSet.GetFileInfo(p));
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            if (!Exists)
            {
                throw new InvalidOperationException("Cannot enumerate a directory that does not exist.");
            }

            return GetFileInfos().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
