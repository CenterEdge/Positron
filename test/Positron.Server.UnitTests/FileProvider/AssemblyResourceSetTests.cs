using System;
using System.IO;
using System.Reflection;
using Positron.Server.FileProvider;
using Xunit;

namespace Positron.Server.UnitTests.FileProvider
{
    public class AssemblyResourceSetTests
    {
        [Fact]
        public void ctor_NullAssembly_ThrowsArgumentNullException()
        {
            // Act

            var ex = Assert.Throws<ArgumentNullException>(() => new AssemblyResourceSet(null));

            // Assert

            Assert.Equal("assembly", ex.ParamName);
        }

        [Fact]
        public void ctor_ThisAssembly_ValidateEntries()
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            var assembly = MethodBase.GetCurrentMethod().DeclaringType.Assembly;

            // Act

            var resourceSet = new AssemblyResourceSet(assembly);

            // Assert

            Assert.Contains(resourceSet.GetDirectoryContents(""), p => p.IsDirectory && p.Exists && p.Name == "fileprovider");
            Assert.Contains(resourceSet.GetDirectoryContents("fileprovider"), p => !p.IsDirectory && p.Exists && p.Name == "test.txt");
        }

        [Fact]
        public void ctor_ThisAssembly_CanReadTestFile()
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            var assembly = MethodBase.GetCurrentMethod().DeclaringType.Assembly;

            var resourceSet = new AssemblyResourceSet(assembly);

            // Act

            string result;
            using (var stream = resourceSet.GetFileInfo("fileprovider/test.txt").CreateReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            // Assert

            Assert.Equal("test", result);
        }

        [Fact]
        public void ctor_ThisAssembly_CanReadTestFileTwice()
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            var assembly = MethodBase.GetCurrentMethod().DeclaringType.Assembly;

            var resourceSet = new AssemblyResourceSet(assembly);

            // Act

            string result;
            using (var stream = resourceSet.GetFileInfo("fileprovider/test.txt").CreateReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            string result2;
            using (var stream = resourceSet.GetFileInfo("fileprovider/test.txt").CreateReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    result2 = reader.ReadToEnd();
                }
            }

            // Assert

            Assert.Equal("test", result);
            Assert.Equal("test", result2);
        }

        [Fact]
        public void ctor_ThisAssemblyFakeFile_DoesNotExist()
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            var assembly = MethodBase.GetCurrentMethod().DeclaringType.Assembly;

            var resourceSet = new AssemblyResourceSet(assembly);

            // Act

            var result = resourceSet.GetFileInfo("fileprovider/fake.txt");

            // Assert

            Assert.False(result.Exists);
        }
    }
}
