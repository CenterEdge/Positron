using System;
using System.Windows;
using Positron.UI.Builder;

namespace Positron.UI
{
    /// <summary>
    /// Provides access to the Positron UI layer after it is built by <see cref="PositronUiBuilder"/>.
    /// </summary>
    public interface IPositronUi : IDisposable
    {
        /// <summary>
        /// Service provider for the UI layer.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Create a new <see cref="PositronWindow"/>.
        /// </summary>
        /// <param name="owner">Owning window, or null for the main window.</param>
        /// <param name="targetUrl">Initial URL to display.</param>
        /// <returns><see cref="PositronWindow"/> ready to be displayed.</returns>
        /// <remarks>
        /// It is up to the caller to configure window (position, size, etc) and call
        /// <see cref="Window.Show"/> or <see cref="Window.ShowDialog"/>.
        /// </remarks>
        PositronWindow CreateWindow(Window owner, string targetUrl);
    }
}
