using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Positron.UI
{
    /// <summary>
    /// Handler for Chromium console messages.
    /// </summary>
    public interface IConsoleLogger
    {
        /// <summary>
        /// Called when Chromium is writing a message to the console.
        /// </summary>
        /// <param name="message">Message being written to the console.</param>
        void WriteMessage(ConsoleMessageEventArgs message);
    }
}
