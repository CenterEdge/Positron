using System;

namespace Positron.Server
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PositronRouteIdentifierAttribute : Attribute
    {
        public string Identifier { get; set; }

        public PositronRouteIdentifierAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}
