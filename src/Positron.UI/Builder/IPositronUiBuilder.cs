using System;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Positron.UI.Builder
{
    public interface IPositronUiBuilder
    {
        IPositronUiBuilder ConfigureServices(Action<IServiceCollection> configureServices);
        IPositronUiBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);
        IPositronUiBuilder SetWebHost(IWebHost webHost);
        IPositronUiBuilder ConfigureSettings(Action<CefSettings> settingsAction);
        IPositronUiBuilder UseConsoleLogger(IConsoleLogger consoleLogger);
        IPositronUiBuilder UseLoggerFactory(ILoggerFactory loggerFactory);
        IWindowHandler Build();
    }
}
