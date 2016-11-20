using System;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Positron.UI.Builder
{
    public interface IPositronUiBuilder
    {
        IPositronUiBuilder ConfigureServices(Action<IServiceCollection> configureServices);
        IPositronUiBuilder SetWebHost(IWebHost webHost);
        IPositronUiBuilder ConfigureSettings(Action<CefSettings> settingsAction);
        IPositronUiBuilder UseConsoleLogger(IConsoleLogger consoleLogger);
        IWindowHandler Build();
    }
}
