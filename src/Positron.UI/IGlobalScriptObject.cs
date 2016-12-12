using Microsoft.Extensions.DependencyInjection;
using Positron.UI.Builder;

namespace Positron.UI
{
    /// <summary>
    /// Represents a script object registered in the global scope for all frames.
    /// </summary>
    /// <remarks>
    /// To extend Chromium with script objects accessible via Javascript, register objects
    /// implementing <see cref="IGlobalScriptObject"/> with the <see cref="ServiceCollection"/>
    /// while building the <see cref="PositronUiBuilder"/>.
    /// <code>
    /// builder.ConfigureServices(services =&lt;
    /// {
    ///     services.AddSingleton&gt;IGlobalScriptObject, ScriptObject&lt;();
    /// })
    /// </code>
    /// </remarks>
    public interface IGlobalScriptObject
    {
        /// <summary>
        /// Name used to register the script object.
        /// </summary>
        string Name { get; }
    }
}
