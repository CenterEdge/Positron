using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using Positron.UI;

namespace Positron.WinFormsApplication
{
    internal class TestLogger : IConsoleLogger
    {
        public void WriteMessage(ConsoleMessageEventArgs message)
        {
            Console.WriteLine("{0}:{1}", message.Line, message.Message);
        }
    }
}
