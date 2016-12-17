using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positron.UI.Dialog
{
    /// <summary>
    /// Extension  of <see cref="DialogContext"/> specific to prompts for user input (window.prompt).
    /// </summary>
    public class PromptDialogContext : DialogContext
    {
        /// <summary>
        /// Before the prompt, has the initial value for the prompt.
        /// It should be filled with the string entered by the user when complete.
        /// </summary>
        public string PromptText { get; set; }
    }
}
