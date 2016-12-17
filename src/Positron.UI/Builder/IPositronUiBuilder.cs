using System;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Positron.UI.Builder
{
    /// <summary>
    /// A builder for <see cref="IPositronUi"/>.
    /// </summary>
    public interface IPositronUiBuilder
    {
        /// <summary>
        /// Builds an <see cref="IPositronUi"/> which hosts a Positron application.
        /// </summary>
        IPositronUi Build();

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggerFactory"/>. This may be called multiple times.
        /// </summary>
        /// <param name="configureLogging">The delegate that configures the <see cref="ILoggerFactory"/>.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        IPositronUiBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);

        /// <summary>
        /// Specify the delegate that is used to configure the services of the UI layer.
        /// </summary>
        /// <param name="configureServices">The delegate that configures the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        IPositronUiBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        /// <summary>
        /// Adds a delegate for configuring the <see cref="CefSettings"/> use for Chromium.  This may be called multiple times.
        /// </summary>
        /// <param name="settingsAction">The delegate that configures the <see cref="CefSettings"/>.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        IPositronUiBuilder ConfigureSettings(Action<CefSettings> settingsAction);

        /// <summary>
        /// Sets the <see cref="IWebHost"/> used to process requests.
        /// </summary>
        /// <param name="webHost"><see cref="IWebHost"/> used to process requests.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        IPositronUiBuilder SetWebHost(IWebHost webHost);

        /// <summary>
        /// Specify the <see cref="ILoggerFactory"/> to be used by the web host.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to be used.</param>
        /// <returns>The <see cref="IPositronUiBuilder"/>.</returns>
        IPositronUiBuilder UseLoggerFactory(ILoggerFactory loggerFactory);
    }
}
