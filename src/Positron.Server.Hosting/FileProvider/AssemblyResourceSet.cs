using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Positron.Server.Hosting.FileProvider
{
    internal class AssemblyResourceSet : IFileProvider
    {
        private readonly Assembly _assembly;
        private ConcurrentDictionary<string, List<string>> _directories;

        public AssemblyResourceSet(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            _assembly = assembly;

            Initialize();
        }

        private void Initialize()
        {
            var directories = new Dictionary<string, List<string>>();

            // Always include the root directory
            directories.Add("", new List<string>());

            var previousDirectory = "";
            var previousFiles = directories[""];

            foreach (var resourceName in GetResourceEntries())
            {
                var slashIndex = resourceName.LastIndexOf('/');
                var directoryName = slashIndex >= 0 ? resourceName.Substring(0, slashIndex) : "";

                List<string> files;
                if (previousDirectory == directoryName)
                {
                    files = previousFiles;
                }
                else
                {
                    files = GetOrCreateDirectory(directories, directoryName);

                    // Remember for next file, improves efficiency
                    previousDirectory = directoryName;
                    previousFiles = files;
                }

                files.Add(resourceName);
            }

            _directories = new ConcurrentDictionary<string, List<string>>(directories);
        }

        private List<string> GetOrCreateDirectory(Dictionary<string, List<string>> directories, string directoryName)
        {
            if (directoryName.Length == 0)
            {
                return directories[""];
            }

            var segments = directoryName.Split('/');

            var previousSegment = directories[""];
            var currentPath = "";
            foreach (var segment in segments)
            {
                if (currentPath.Length > 0)
                {
                    currentPath += "/";
                }
                currentPath += segment;

                List<string> files;
                if (!directories.TryGetValue(currentPath, out files))
                {
                    // Add the directory to the parent directory
                    previousSegment.Add(currentPath);

                    // Add the directory to the directory list
                    previousSegment = new List<string>();
                    directories.Add(currentPath, previousSegment);
                }
                else
                {
                    previousSegment = files;
                }
            }

            return previousSegment;
        }

        private IEnumerable<string> GetResourceEntries()
        {
            string resourceName = _assembly.GetName().Name + ".g.resources";
            using (var stream = _assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return new string[] {};
                }

                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>()
                        .Select(entry => (string)entry.Key)
                        .OrderBy(p => p)
                        .ToList();
                }
            }
        }

        private Uri BuildUri(string subpath)
        {
            // Use PackUriHelper to ensure that it has initialized Uri to accept pack uris

            return
                new Uri(
                    $"{PackUriHelper.UriSchemePack}://application:,,,/{_assembly.GetName().Name};component/{subpath}");
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                throw new ArgumentNullException(nameof(subpath));
            }

            var uri = BuildUri(subpath);
            var name = Path.GetFileName(uri.GetComponents(UriComponents.Path, UriFormat.Unescaped));

            List<string> files;
            if (_directories.TryGetValue(subpath, out files))
            {
                return new ResourceFileInfo(_assembly, uri, name, null, true);
            }

            try
            {
                var streamResourceInfo = Application.GetResourceStream(uri);
                return new ResourceFileInfo(_assembly, uri, name, streamResourceInfo, false);
            }
            catch (IOException)
            {
                return new ResourceFileInfo(_assembly, uri, name, null, false);
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            List<string> files;
            if (_directories.TryGetValue(subpath, out files))
            {
                return new ResourceDirectoryContents(this, files);
            }
            else
            {
                return new ResourceDirectoryContents(this, null);
            }
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}
