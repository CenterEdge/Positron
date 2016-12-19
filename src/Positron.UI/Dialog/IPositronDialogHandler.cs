using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positron.UI.Dialog
{
    /// <summary>
    /// Handler for Javascript dialog events such as window.alert(), window.confirm(), and window.prompt().
    /// Allows customization of application behavior for these dialogs.
    /// </summary>
    public interface IPositronDialogHandler
    {
        /// <summary>
        /// Returns true if the handler will handle the alert request.
        /// </summary>
        /// <param name="context">Information about the requested alert.</param>
        /// <returns>True if the handler will handle the alert request, suppressing the default behavior.</returns>
        /// <remarks>May also see <see cref="DialogContext.SuppressDefaultDialog"/> to prevent any UI from appearing.</remarks>
        bool WillHandleAlert(DialogContext context);

        /// <summary>
        /// Returns true if the handler will handle the confirmation request.
        /// </summary>
        /// <param name="context">Information about the requested confirmation.</param>
        /// <returns>True if the handler will handle the confirmation request, suppressing the default behavior.</returns>
        /// <remarks>May also see <see cref="DialogContext.SuppressDefaultDialog"/> to prevent any UI from appearing.</remarks>
        bool WillHandleConfirmation(DialogContext context);

        /// <summary>
        /// Returns true if the handler will handle the prompt request.
        /// </summary>
        /// <param name="context">Information about the requested prompt.</param>
        /// <returns>True if the handler will handle the prompt request, suppressing the default behavior.</returns>
        /// <remarks>May also see <see cref="DialogContext.SuppressDefaultDialog"/> to prevent any UI from appearing.</remarks>
        bool WillHandlePrompt(PromptDialogContext context);

        /// <summary>
        /// Displays an alert to the user.
        /// </summary>
        /// <param name="context">Information about the requested alert.</param>
        /// <returns>Task which completes after OK is pressed by the user.</returns>
        Task HandleAlert(DialogContext context);

        /// <summary>
        /// Displays a prompt for user confirmation.
        /// </summary>
        /// <param name="context">Information about the requested confirmation.</param>
        /// <returns>Task with true for yes or false for no.</returns>
        Task<bool> HandleConfirmation(DialogContext context);

        /// <summary>
        /// Displays a prompt for user input.  Input should be stored in <see cref="PromptDialogContext.PromptText"/>.
        /// </summary>
        /// <param name="context">Information about the requested prompt.</param>
        /// <returns>Task with true for success or false for user cancellation.</returns>
        Task<bool> HandlePrompt(PromptDialogContext context);
    }
}
