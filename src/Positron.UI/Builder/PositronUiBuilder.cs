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
    public class PositronUiBuilder : IPositronUiBuilder
    {
        private IWebHost _webHost;
        private IConsoleLogger _consoleLogger;
        private ILoggerFactory _loggerFactory;
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates =
            new List<Action<IServiceCollection>>();
        private readonly List<Action<ILoggerFactory>> _configureLoggingDelegates =
            new List<Action<ILoggerFactory>>();

        private bool _isBuilt;

        public IPositronUiBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public IPositronUiBuilder SetWebHost(IWebHost webHost)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            _webHost = webHost;
            return this;
        }

        public IPositronUiBuilder UseConsoleLogger(IConsoleLogger consoleLogger)
        {
            _consoleLogger = consoleLogger;
            return this;
        }

        public IPositronUiBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        public IPositronUiBuilder ConfigureSettings(Action<CefSettings> settingsAction)
        {
            _configureServicesDelegates.Add(services =>
            {
                services.Configure(settingsAction);
            });

            return this;
        }

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggerFactory"/>. This may be called multiple times.
        /// </summary>
        /// <param name="configureLogging">The delegate that configures the <see cref="ILoggerFactory"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public IPositronUiBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            if (configureLogging == null)
            {
                throw new ArgumentNullException(nameof(configureLogging));
            }

            _configureLoggingDelegates.Add(configureLogging);
            return this;
        }

        public IWindowHandler Build()
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

            return serviceProvider.GetService<IWindowHandler>();
        }

        public IPositronUiBuilder UseDebugPort(int debugPort)
        {
            return ConfigureSettings((settings) =>
            {
                settings.RemoteDebuggingPort = debugPort;

            });
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
            services.TryAddSingleton<IWindowHandler, WindowHandler>();
            services.TryAddSingleton<ILifeSpanHandler, LifeSpanHandler>();
            services.TryAddSingleton<IKeyboardHandler, KeyboardHandler>();

            services.TryAddSingleton(_webHost.Services.GetService<IAppSchemeResourceResolver>());
            services.AddSingleton(_webHost);
            if (_consoleLogger != null)
            {
                services.AddSingleton(_consoleLogger);
            }
            else
            {
                services.AddSingleton<IConsoleLogger>(new NullConsoleLogger());
            }

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }

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
