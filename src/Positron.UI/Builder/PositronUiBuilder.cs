using System;
using System.Collections.Generic;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Positron.Server;
using System.Configuration;

namespace Positron.UI.Builder
{
    public class PositronUiBuilder : IPositronUiBuilder
    {
        private IWebHost _webHost;
        private IConsoleLogger _consoleLogger;
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates =
            new List<Action<IServiceCollection>>();

        private bool _isBuilt;
        private CefSettings _cefSettings;

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

        public IPositronUiBuilder ConfigureSettings(Action<CefSettings> settingsAction)
        {
            _configureServicesDelegates.Add(services =>
            {
                services.Configure(settingsAction);
            });

            return this;
        }

        public IWindowHandler Build(int? debugPort = null)
        {
            if (_isBuilt)
            {
                throw new InvalidOperationException($"Can only use the {nameof(PositronUiBuilder)} once.");
            }
            if (_webHost == null)
            {
                throw new InvalidOperationException("An IWebHost must be provided via SetWebHost.");
            }

            var services = BuildServices();
            var serviceProvider = services.BuildServiceProvider();
            var settings = new CefSettings();

            foreach (var options in serviceProvider.GetServices<IConfigureOptions<CefSettings>>())
            {
                options.Configure(settings);
            }

#if DEBUG
            if (debugPort.HasValue)
            {
                settings.RemoteDebuggingPort = debugPort.Value;
            }
#endif

            Cef.Initialize(settings, true, serviceProvider.GetService<IBrowserProcessHandler>());

            _isBuilt = true;

            return serviceProvider.GetService<IWindowHandler>();
        }

        private IServiceCollection BuildServices()
        {
            var services = new ServiceCollection();

            services.AddOptions();
            services.Configure<CefSettings>(DefaultCefSettings);

            services.TryAddSingleton<IBrowserProcessHandler, BrowserProcessHandler>();
            services.TryAddSingleton<IRequestHandler, RequestHandler>();
            services.TryAddSingleton<IWindowHandler, WindowHandler>();
            services.TryAddSingleton<ILifeSpanHandler, LifeSpanHandler>();

            services.TryAddSingleton(_webHost.Services.GetService<IAppSchemeResourceResolver>());
            services.AddSingleton(_webHost);
            services.AddSingleton(_consoleLogger);


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

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "app",
                IsLocal = false,
                IsDisplayIsolated = false,
                IsStandard = false,
                SchemeHandlerFactory = new AppSchemeHandlerFactory(_webHost)
            });
        }


    }
}
