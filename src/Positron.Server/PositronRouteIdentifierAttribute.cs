using System;

namespace Positron.Server
{
    /// <summary>
    /// Provides a specific identifier to use for an assembly in Positron URIs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PositronRouteIdentifierAttribute : Attribute
    {
        /// <summary>
        /// Identifier to use for the assembly in Positron URIs.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Creates a new <see cref="PositronRouteIdentifierAttribute"/>.
        /// </summary>
        /// <param name="identifier">Identifier to use for the assembly in Positron URIs.</param>
        public PositronRouteIdentifierAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}
