using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Positron.UI.Dialog;

namespace Positron.UI.Internal
{
    internal class PositronJsDialogHandler : IJsDialogHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PositronJsDialogHandler> _logger;

        public PositronJsDialogHandler(IServiceProvider serviceProvider, ILogger<PositronJsDialogHandler> logger)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType,
            string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            try
            {
                var wpfBrowser = browserControl as ChromiumWebBrowser;
                var window = wpfBrowser?.Parent as PositronWindow;
                if (window == null)
                {
                    // Ignore if not within a Positron window
                    callback.Dispose();
                    suppressMessage = false;
                    return false;
                }

                DialogContext context = dialogType == CefJsDialogType.Prompt
                    ? new PromptDialogContext
                    {
                        PromptText = defaultPromptText
                    }
                    : new DialogContext();

                context.Window = window;
                context.OriginUrl = new Uri(originUrl);
                context.MessageText = messageText;

                var dialogHandler = _serviceProvider.GetService<IPositronDialogHandler>();
                if (dialogHandler == null)
                {
                    // Use default behavior
                    callback.Dispose();
                    suppressMessage = false;
                    return false;
                }

                bool handled;
                switch (dialogType)
                {
                    case CefJsDialogType.Alert:
                        handled = HandleAlert(dialogHandler, context, callback);
                        break;

                    case CefJsDialogType.Confirm:
                        handled = HandleConfirmation(dialogHandler, context, callback);
                        break;

                    case CefJsDialogType.Prompt:
                        handled = HandlePrompt(dialogHandler, (PromptDialogContext) context, callback);
                        break;

                    default:
                        handled = false;
                        _logger.LogWarning(LoggerEventIds.UnknownDialogType, "Unknown dialog type: {0}", dialogType);
                        break;
                }

                suppressMessage = !handled && context.SuppressDefaultDialog;
                return handled;
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerEventIds.DialogHandlerError, ex, "Error in IPositronDialogHandler");

                callback.Dispose();
                suppressMessage = false;
                return false;
            }
        }

        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload,
            IJsDialogCallback callback)
        {
            // Use default behavior
            return false;
        }

        public void OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public void OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
        }

        private bool HandleAlert(IPositronDialogHandler handler, DialogContext context, IJsDialogCallback callback)
        {
            if (!handler.WillHandleAlert(context))
            {
                return false;
            }

            var task = handler.HandleAlert(context)
                .ContinueWith(task2 =>
                {
                    using (callback)
                    {
                        callback.Continue(true);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            HandleErrors(task, callback);

            return true;
        }

        private bool HandleConfirmation(IPositronDialogHandler handler, DialogContext context, IJsDialogCallback callback)
        {
            if (!handler.WillHandleConfirmation(context))
            {
                return false;
            }

            var task = handler.HandleConfirmation(context)
                .ContinueWith(task2 =>
                {
                    using (callback)
                    {
                        callback.Continue(task2.Result);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            HandleErrors(task, callback);

            return true;
        }

        private bool HandlePrompt(IPositronDialogHandler handler, PromptDialogContext context, IJsDialogCallback callback)
        {
            if (!handler.WillHandlePrompt(context))
            {
                return false;
            }

            var task = handler.HandlePrompt(context)
                .ContinueWith(task2 =>
                {
                    using (callback)
                    {
                        callback.Continue(task2.Result, context.PromptText);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            HandleErrors(task, callback);

            return true;
        }

        /// <summary>
        /// Adds generic ContinueWith for errors and cancellations.
        /// </summary>
        private void HandleErrors(Task task, IJsDialogCallback callback)
        {
            task.ContinueWith(task2 =>
            {
                using (callback)
                {
                    if (task2.IsFaulted)
                    {
                        _logger.LogError(LoggerEventIds.DialogHandlerError, task2.Exception,
                            "Error in IPositronDialogHandler");

                        callback.Continue(false);
                    }
                    else
                    {
                        _logger.LogError(LoggerEventIds.DialogHandlerError, "Cancellation in IPositronDialogHandler");

                        callback.Continue(false);
                    }
                }
            }, TaskContinuationOptions.NotOnRanToCompletion);
        }
    }
}
