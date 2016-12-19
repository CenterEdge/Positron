using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Positron.UI.Dialog
{
    /// <summary>
    /// Information about a Javascript request for dialog, such as window.alert().
    /// </summary>
    public class DialogContext
    {
        /// <summary>
        /// Text of the message to display.
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Origin URL which requested the dialog.
        /// </summary>
        public Uri OriginUrl { get; set; }

        /// <summary>
        /// <see cref="PositronWindow"/> which requested the dialog.
        /// </summary>
        public PositronWindow Window { get; set; }

        /// <summary>
        /// If set to true during a call to WillHandleXXX in <see cref="IPositronDialogHandler"/>,
        /// and if WillHandleXXX returns false, it will also suppress the default dialog.
        /// </summary>
        public bool SuppressDefaultDialog { get; set; } = false;
    }
}
