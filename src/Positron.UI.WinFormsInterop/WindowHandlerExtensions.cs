using System.Windows.Interop;

namespace Positron.UI.WinFormsInterop
{
    /// <summary>
    /// Provides extensions to <see cref="IWindowHandler"/>.
    /// </summary>
    public static class WindowHandlerExtensions
    {
        /// <summary>
        /// Create a new <see cref="PositronWindow"/> with a WinForms window as this parent.
        /// </summary>
        /// <param name="windowHandler"><see cref="IWindowHandler"/> used to create the window.</param>
        /// <param name="owner">WinForms owner.</param>
        /// <param name="targetUrl">Initial URL to display on load.</param>
        /// <returns>New <see cref="PositronWindow"/>, ready to receive a ShowDialog call.</returns>
        public static PositronWindow CreateWindowFromWinForms(this IWindowHandler windowHandler,
            System.Windows.Forms.IWin32Window owner, string targetUrl)
        {
            var window = windowHandler.CreateWindow(null, targetUrl);

            if (owner != null)
            {
                new WindowInteropHelper(window).Owner = owner.Handle;
            }

            return window;
        }
    }
}
