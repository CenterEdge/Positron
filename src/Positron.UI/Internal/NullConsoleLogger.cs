using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Positron.UI.Internal
{
    internal class NullConsoleLogger : IConsoleLogger
    {
        public void WriteMessage(ConsoleMessageEventArgs message)
        {
        }
    }
}
