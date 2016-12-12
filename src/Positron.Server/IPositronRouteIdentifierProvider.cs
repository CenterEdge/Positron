using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Positron.Server
{
    /// <summary>
    /// Provides information about assembly identities, based upon <see cref="PositronRouteIdentifierAttribute" />.
    /// </summary>
    public interface IPositronRouteIdentifierProvider
    {
        /// <summary>
        /// Returns an <see cref="AssemblyPart"/> for a given identifier.
        /// </summary>
        /// <param name="identifier">Identifier to resolve.</param>
        /// <returns><see cref="ApplicationPart"/> for the identifier, or null if none was found.</returns>
        AssemblyPart GetApplicationPart(string identifier);

        /// <summary>
        /// Returns the identifier for a give <see cref="AssemblyPart"/>.
        /// </summary>
        /// <param name="assemblyName"><see cref="AssemblyPart"/> to resolve.</param>
        /// <returns>Identifier for the <see cref="AssemblyPart"/>.</returns>
        string GetIdentifier(AssemblyName assemblyName);
    }
}
