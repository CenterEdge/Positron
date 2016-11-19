using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Positron.Server
{
    public interface IAssemblyIdentifierProvider
    {
        AssemblyPart GetApplicationPart(string identifier);
        string GetIdentifier(AssemblyName assemblyName);
    }
}
