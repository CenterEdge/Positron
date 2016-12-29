using System;
using CefSharp;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Positron.UI.Internal
{
    internal class KeyboardHandler : IKeyboardHandler
    {
        private readonly ILogger<KeyboardHandler> _logger;
        private const int ControlRCode = 18;
        private const int F12Code = 123;

        private CefSettings Settings { get; }

        public KeyboardHandler(CefSettings settings, ILogger<KeyboardHandler> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Settings = settings;
            _logger = logger;
        }

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            try
            {
                var isHandled = false;

                switch (type)
                {
                    case KeyType.RawKeyDown:
                        if (windowsKeyCode == F12Code && Settings.RemoteDebuggingPort != default(int))
                        {
                            _logger.LogInformation(LoggerEventIds.OpenDebugTools, "F12: Opening debug tools");

                            Process.Start("chrome.exe", "http://localhost:" + Settings.RemoteDebuggingPort);
                            isHandled = true;
                        }
                        break;
                    case KeyType.KeyUp:
                        break;
                    case KeyType.Char:
                        if (windowsKeyCode == ControlRCode)
                        {
                            _logger.LogInformation(LoggerEventIds.Refresh, "Ctrl-R: Triggering reload of '{0}'",
                                browser.MainFrame?.Url ?? "unknown");

                            browserControl.Reload();
                            isHandled = true;
                        }
                        break;
                }

                return isHandled;
            }
            catch (Exception ex)
            {
                // Unhandled exceptions may cause app crash, so trap and log
                _logger.LogError(LoggerEventIds.UnhandledError, ex, Resources.LogMessage_Unhandled_Error);

                return false;
            }
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            var isHandled = false;

            switch (type)
            {
                case KeyType.RawKeyDown:
                    if (windowsKeyCode == F12Code)
                    {
                        isKeyboardShortcut = true;
                    }
                    break;
                case KeyType.KeyUp:
                    break;
                case KeyType.Char:
                    if (windowsKeyCode == ControlRCode)
                    {
                        isKeyboardShortcut = true;
                    }
                    break;
            }

            return isHandled;
        }
    }
}
