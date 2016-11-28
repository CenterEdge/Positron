using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;

namespace Positron.UI
{
    public class KeyboardHandler : IKeyboardHandler
    {
        private const int ControlRCode = 18;

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            var isHandled = false;

            switch (type)
            {
                case KeyType.RawKeyDown:
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

        private void HandleChar()
        {

        }


        private void RefreshPage()
        {

        }
    }
}
