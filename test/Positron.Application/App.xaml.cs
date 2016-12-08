using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using CefSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Positron.Server.Hosting;
using Positron.UI;
using Positron.UI.Builder;

namespace Positron.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        [STAThread]
        [System.Diagnostics.DebuggerNonUserCode]
        public static void Main()
        {
#if DEBUG
            var environmentName = "Development";
#else
            var environmentName = "Production";
#endif

            var builder = new WebHostBuilder()
                .UseEnvironment(environmentName)
                .UsePositronServer()
                .UseStartup<Startup>();

            var webHost = builder.Build();
            try
            {
                webHost.Start();
                LoadApp(webHost);
            }
            finally
            {
                webHost.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadApp(IWebHost webHost)
        {
            var app = new App();
            app.InitializeComponent();

            var uiBuilder = new PositronUiBuilder()
                .SetWebHost(webHost)
                .UseDebugPort(8080)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IGlobalScriptObject, TestScriptObject>();
                });

            var windowHandler = uiBuilder.Build();
            app.Run(windowHandler.CreateWindow((Window)null, "http://positron/Positron.Application"));
        }
    }
}
