using System;

namespace Positron.Server
{
    public interface IAppSchemeResourceResolver
    {
        Uri GetResourceUri(string input);
    }
}