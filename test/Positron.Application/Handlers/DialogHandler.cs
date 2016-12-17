using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Positron.UI.Dialog;

namespace Positron.Application.Handlers
{
    public class DialogHandler : IPositronDialogHandler
    {
        public bool WillHandleAlert(DialogContext context)
        {
            return true;
        }

        public bool WillHandleConfirmation(DialogContext context)
        {
            return true;
        }

        public bool WillHandlePrompt(PromptDialogContext context)
        {
            return false;
        }

        public Task HandleAlert(DialogContext context)
        {
            var operation = context.Window.Dispatcher.InvokeAsync(
                () => MessageBox.Show(context.Window, context.MessageText, "Alert", MessageBoxButton.OK,
                    MessageBoxImage.Information));

            return operation.Task;
        }

        public async Task<bool> HandleConfirmation(DialogContext context)
        {
            var operation = context.Window.Dispatcher.InvokeAsync(
                () => MessageBox.Show(context.Window, context.MessageText, "Confirmation", MessageBoxButton.YesNo,
                    MessageBoxImage.Question));

            var result = await operation.Task;

            return result == MessageBoxResult.Yes;
        }

        public Task<bool> HandlePrompt(PromptDialogContext context)
        {
            throw new NotImplementedException();
        }
    }
}
