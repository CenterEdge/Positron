using System;
using System.Windows;

namespace Positron.UI
{
    public interface IWindowHandler
    {
        IServiceProvider Services { get; }
        PositronWindow CreateWindow(Window owner, string targetUrl);
    }
}
