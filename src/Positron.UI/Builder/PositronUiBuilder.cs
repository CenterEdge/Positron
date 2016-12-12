using System;
using System.Collections.Generic;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Positron.Server;
using Positron.UI.Internal;

namespace Positron.UI.Builder
{
    /// <summary>
    /// A builder for <see cref="IPositronUi"/>.
    /// </summary>
    public class PositronUiBuilder : IPositronUiBuilder
    {
        private IWebHost _webHost;
        private ILoggerFactory _loggerFactory;
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates =
            new List<Action<IServiceCollection>>();
        private readonly List<Action<ILoggerFactory>> _configureLoggingDelegates =
            new List<Action<ILoggerFactory>>();

        private bool _isBuilt;

        /// <inheritdoc cref="IPositronUiBuilder"/>
        public IPositronUiBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        /// <inheritdoc cref="IPositronUiBuilder"/>
        public IPositronUiBuilder SetWebHost(IWebHost webHost)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            _webHost = webHost;
            return this;
        }

        /// <inheritdoc cref="IPositronUiBuilder"/>
        public IPositronUiBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <inheritdoc cref="IPositronUiBuilder"/>
        public IPositronUiBuilder ConfigureSettings(Action<CefSettings> settingsAction)
        {
            _configureServicesDelegates.Add(services =>
            {
                services.Configure(settingsAction);
            });

            return this;
        }

        /// <inheritdoc cref="IPositronUiBuilder"/>
        public IPositronUiBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            if (configureLogging == null)
            {
                throw new ArgumentNullException(nameof(configureLogging));
            }

            _configureLoggingDelegates.Add(configureLogging);
            return this;
        }

        /// <inheritdoc cref="IPositronUiBuilder"/>
        public IPositronUi Build()
        {
            if (_isBuilt)
            {
                throw new InvalidOperationException($"Can only use the {nameof(PositronUiBuilder)} once.");
            }
            if (_webHost == null)
            {
                throw new InvalidOperationException("An IWebHost must be provided via SetWebHost.");
            }

            var settings = new CefSettings();
            var services = BuildServices(settings);
            var serviceProvider = services.BuildServiceProvider();

            foreach (var options in serviceProvider.GetServices<IConfigureOptions<CefSettings>>())
            {
                options.Configure(settings);
            }

            Cef.Initialize(settings, true, serviceProvider.GetService<IBrowserProcessHandler>());

            _isBuilt = true;

            return serviceProvider.GetService<IPositronUi>();
        }

        private IServiceCollection BuildServices(CefSettings settings)
        {
            var services = new ServiceCollection();

            services.AddOptions();
            services.Configure<CefSettings>(DefaultCefSettings);

            // These settings will be updated later
            // Registering here will allow the final settings to be injected if needed
            services.TryAddSingleton(settings);

            if (_loggerFactory == null)
            {
                // By default, if we haven't configured a specific logger factory then reuse the one from IWebHost
                _loggerFactory = _webHost.Services.GetRequiredService<ILoggerFactory>();
                services.AddSingleton(provider => _loggerFactory);
            }
            else
            {
                services.AddSingleton(_loggerFactory);
            }

            foreach (var configureLogging in _configureLoggingDelegates)
            {
                configureLogging(_loggerFactory);
            }

            //This is required to add ILogger of T.
            services.AddLogging();

            services.TryAddSingleton<IBrowserProcessHandler, BrowserProcessHandler>();
            services.TryAddSingleton<IRequestHandler, RequestHandler>();
            services.TryAddSingleton<IResourceHandlerFactory, PositronResourceHandlerFactory>();
            services.TryAddSingleton<IPositronUi, PositronUi>();
            services.TryAddSingleton<ILifeSpanHandler, LifeSpanHandler>();
            services.TryAddSingleton<IKeyboardHandler, KeyboardHandler>();

            services.TryAddSingleton(_webHost.Services.GetService<IPositronResourceResolver>());
            services.AddSingleton(_webHost);

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }

            // This will be used only if an IConsoleLogger hasn't already been registered
            services.TryAddSingleton<IConsoleLogger, NullConsoleLogger>();

            return services;
        }

        private void DefaultCefSettings(CefSettings settings)
        {
            settings.WindowlessRenderingEnabled = true;
            settings.MultiThreadedMessageLoop = true;
            settings.FocusedNodeChangedEnabled = true;

            settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1");

            settings.SetOffScreenRenderingBestPerformanceArgs();
        }
    }
}
