using CefSharp;
using System.Diagnostics;


namespace Positron.UI
{
    public class KeyboardHandler : IKeyboardHandler
    {
        private const int ControlRCode = 18;
        private const int F12Code = 123;

        private CefSettings Settings { get; }

        public KeyboardHandler(CefSettings settings)
        {
            Settings = settings;
        }

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            var isHandled = false;


            switch (type)
            {
                case KeyType.RawKeyDown:
                    if (windowsKeyCode == F12Code && Settings.RemoteDebuggingPort != default(int))
                    {
                        Process.Start("chrome.exe", "http://localhost:" + Settings.RemoteDebuggingPort);
                        isHandled = true;
                    }
                    break;
                case KeyType.KeyUp:
                    break;
                case KeyType.Char:
                    if (windowsKeyCode == ControlRCode)
                    {
                        browserControl.Reload();
                        isHandled = true;
                    }
                    break;
            }

            return isHandled;
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
