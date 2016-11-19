using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Positron.Server.Utils;

namespace Positron.Server
{
    public class PositronAssemblyIdentifierProvider : IAssemblyIdentifierProvider
    {
        private readonly ConcurrentDictionary<string, AssemblyPart> _cacheByIdentifier = new ConcurrentDictionary<string, AssemblyPart>(new CaseInsenstiveEqualityComparer());
        private readonly ConcurrentDictionary<string, string> _cacheByAssemblyName = new ConcurrentDictionary<string, string>();
        private readonly ApplicationPartManager _applicationPartManager;

        public PositronAssemblyIdentifierProvider(ApplicationPartManager applicationPartManager)
        {
            if (applicationPartManager == null)
            {
                throw new ArgumentNullException(nameof(applicationPartManager));
            }

            _applicationPartManager = applicationPartManager;

            LoadAssemblies();
        }

        public AssemblyPart GetApplicationPart(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            AssemblyPart part;
            if (_cacheByIdentifier.TryGetValue(identifier, out part))
            {
                return part;
            }
            else
            {
                return null;
            }
        }

        public string GetIdentifier(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                return null;
            }

            string identifier;
            if (_cacheByAssemblyName.TryGetValue(assemblyName.FullName, out identifier))
            {
                return identifier;
            }
            else
            {
                return null;
            }
        }

        private void LoadAssemblies()
        {
            foreach (var part in _applicationPartManager.ApplicationParts.OfType<AssemblyPart>())
            {
                var identifier = ExtractIdentifier(part.Assembly);
                _cacheByIdentifier.TryAdd(identifier, part);
                _cacheByAssemblyName.TryAdd(part.Assembly.FullName, identifier);
            }
        }

        private string ExtractIdentifier(Assembly assembly)
        {
            return (assembly.GetCustomAttribute<PositronRouteIdentifierAttribute>()?.Identifier ??
                assembly.GetName().Name)
                .ToLowerInvariant();
        }
    }
}
