namespace Positron.UI
{
    /// <summary>
    /// Represents a script object registered in the global scope for all frames.
    /// </summary>
    public interface IGlobalScriptObject
    {
        /// <summary>
        /// Name used to register the script object.
        /// </summary>
        string Name { get; }
    }
}
