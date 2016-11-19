using System;
using System.IO;
using System.Reflection;
using System.Windows.Resources;
using Microsoft.Extensions.FileProviders;

namespace Positron.Server.FileProvider
{
    internal class ResourceFileInfo : IFileInfo
    {
        private readonly Assembly _assembly;
        private readonly Uri _uri;
        private readonly string _name;
        private readonly StreamResourceInfo _streamResourceInfo;

        public bool Exists => _streamResourceInfo != null || IsDirectory;
        public long Length => _streamResourceInfo?.Stream.Length ?? -1;
        public string PhysicalPath => _uri?.ToString();
        public string Name => _name;
        public DateTimeOffset LastModified =>
            _assembly?.Location != null
                ? new DateTimeOffset(File.GetLastWriteTime(_assembly.Location))
                : DateTimeOffset.MinValue;
        public bool IsDirectory { get; }

        public string MimeType => _streamResourceInfo?.ContentType;

        public ResourceFileInfo(Assembly assembly, Uri uri, string name, StreamResourceInfo streamResourceInfo, bool isDirectory)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _assembly = assembly;
            _uri = uri;
            _name = name;
            _streamResourceInfo = streamResourceInfo;
            IsDirectory = isDirectory;
        }

        public Stream CreateReadStream()
        {
            if (IsDirectory)
            {
                throw new InvalidOperationException("Cannot call CreateReadStream on a directory.");
            }
            if (_streamResourceInfo == null)
            {
                throw new InvalidOperationException("Cannot call CreateReadStream on a file which does not exist.");
            }

            return _streamResourceInfo.Stream;
        }
    }
}
