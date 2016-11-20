using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Positron.UI
{
    public interface IConsoleLogger
    {
        void WriteMessage(ConsoleMessageEventArgs message);
    }
}
